using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.DTO;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;


namespace WebApplication1.Service.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IConfiguration _config;
        private readonly IEmailService emailService;
        private readonly ISmsService smsService;
        private readonly IForgotPasswordRepository forgotPasswordRepository;
        private readonly IHttpContextAccessor httpContextAccessor; // ip address access


        public AuthService(IUserRepository userRepository, IConfiguration configuration, IForgotPasswordRepository forgotPasswordRepository, IEmailService emailService , IHttpContextAccessor httpContextAccessor, ISmsService smsService)
        {
            this.userRepository = userRepository;
            this._config = configuration;
            this.forgotPasswordRepository = forgotPasswordRepository;
            this.emailService = emailService;
            this.httpContextAccessor = httpContextAccessor;
            this.smsService = smsService;
        }



        public async Task<ForgotPasswordResponseDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userRepository.GetUserByEmail(forgotPasswordDto.Email);

            if(user == null)
            {
                return new ForgotPasswordResponseDto
                {
                    Token = "If the email is registered, a password reset link has been sent."
                };
            }

            

            var recentRequestsCount = await forgotPasswordRepository.GetRecentRequestsCountAsync(user.Id,DateTime.UtcNow.AddHours(-1));

            if (recentRequestsCount >= 3)
            {
               
                return new ForgotPasswordResponseDto
                {
                    Token = "If the email is registered, a password reset link has been sent."
                };
            }

            
            var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            
            
            using var sha256 = SHA256.Create();
            var tokenHash = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken)));

            var clientIp = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP";

            var resetTokenEntry = new ForgotPassword
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15), 
                IsUsed = false,
                AttemptsCount = 0,
                RequestedIp = clientIp,
                CreatedAt = DateTime.UtcNow
            };

            System.Diagnostics.Debug.WriteLine("Raw token", rawToken);

            var resetLink = $"https://yourfrontendapp.com/reset-password?token={rawToken}";

            var emailBody = $@"
                            <h3>Password Reset Request</h3>
                            <p>ඔබගේ ගිණුමේ මුරපදය reset කිරීමට ඉල්ලීමක් ලැබී ඇත. පහත link එක click කර නව මුරපදයක් සකසන්න:</p>
                            <p><a href='{resetLink}'>Reset Password</a></p>
                            <p>මෙම link එක වලංගු වන්නේ විනාඩි 15ක් පමණි.</p>";

            await forgotPasswordRepository.InvalidatePreviousTokensAsync(user.Id);

            try
            {
                await emailService.SendEmailAsync(user.Profile.Email, "Reset Your Password", emailBody);

                await forgotPasswordRepository.AddAsync(resetTokenEntry);
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine($"Email send failed: {ex.Message}");

                return new ForgotPasswordResponseDto
                {
                    Token = "Failed to send email. Please try again later."
                };
            }


            return new ForgotPasswordResponseDto
            {
                Token = "Password reset link sent to your email."
            };
        }






        public async Task<AuthLoginResponseDto> Login(AuthLoginDto authLoginDto)
        {
            if (string.IsNullOrEmpty(authLoginDto.username) || string.IsNullOrEmpty(authLoginDto.password))
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please check your username and password", 
                    Token = null 
                };
            }

            var userCheck = await userRepository.GetUserByUsername(authLoginDto.username);

            if (userCheck == null)
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please Check Your Username and Password", 
                    Token = null 
                };
            }

            if (userCheck.Credential.LockoutUntil.HasValue && userCheck.Credential.LockoutUntil.Value > DateTime.UtcNow)
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Account Locked", 
                    Token = null 
                };
            }

            if (userCheck.Status != AccountStatus.Active)
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please Verify your mobile and email first", 
                    Token = null 
                };
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(authLoginDto.password, userCheck.Credential.PasswordHash);

            if (!isPasswordValid)
            {
                userCheck.Credential.FailedLoginAttempts++;

                if (userCheck.Credential.FailedLoginAttempts >= 5)
                {
                    userCheck.Credential.LockoutUntil = DateTime.UtcNow.AddMinutes(2);
                    await userRepository.SaveUser(userCheck);
                    return new AuthLoginResponseDto { 
                        Success = false, 
                        Message = "Acccount Lock in 5 Minutes", 
                        Token = null 
                    };
                }

                await userRepository.SaveUser(userCheck);
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please Check Your Username or Password", 
                    Token = null 
                };
            }

            userCheck.Credential.FailedLoginAttempts = 0;
            userCheck.Credential.LockoutUntil = null;
            await userRepository.SaveUser(userCheck);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                                        issuer: _config["Jwt:Issuer"],
                                        audience: _config["Jwt:Audience"],
                                        claims: new[]
                                        {
                                    new Claim(ClaimTypes.Name, userCheck.Username),
                                    new Claim(ClaimTypes.NameIdentifier, userCheck.Id.ToString()),
                                    new Claim(ClaimTypes.Role, userCheck.Role.ToString())
                                        },
                                        expires: DateTime.UtcNow.AddHours(1),
                                        signingCredentials: new SigningCredentials(
                                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                                            SecurityAlgorithms.HmacSha256)
            ));


            

            return new AuthLoginResponseDto { 
                Success = true, 
                Message = "Login Successfull", 
                Token = token 
            };
        }

        public async Task<bool> ResetPassword(ResetPasswordDto dto)
        {
            
            using var sha256 = SHA256.Create();
            var incomingHash = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(dto.Token)));


            var resetTokenEntry = await forgotPasswordRepository.GetByTokenHashAsync(incomingHash);

            if (resetTokenEntry == null)
            {
                resetTokenEntry.AttemptsCount++; 
                await forgotPasswordRepository.UpdateAsync(resetTokenEntry); 
                return false;
            }

            if (resetTokenEntry.AttemptsCount >= 3)
            {
                resetTokenEntry.IsUsed = true;
                await forgotPasswordRepository.UpdateAsync(resetTokenEntry);
                return false;
            }

            var currentIp = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(resetTokenEntry.RequestedIp) && resetTokenEntry.RequestedIp != currentIp)
            {
                // IP වෙනස් නම් එය අසාර්ථක උත්සාහයක් ලෙස සලකා AttemptsCount වැඩි කිරීම
                resetTokenEntry.AttemptsCount++;

                // සැකකටයුතු උත්සාහයක් බැවින් Token එක ක්ෂණිකව අවලංගු කිරීමටද හැක:
                 resetTokenEntry.IsUsed = true;

                await forgotPasswordRepository.UpdateAsync(resetTokenEntry);
                return false;
            }

                        
            if (resetTokenEntry.IsUsed || resetTokenEntry.ExpiresAt < DateTime.UtcNow)
            {
                resetTokenEntry.AttemptsCount++;
                resetTokenEntry.IsUsed = true;
                await forgotPasswordRepository.UpdateAsync(resetTokenEntry);
                return false;
            }

            

            var user = await userRepository.GetUserByIdAsync(resetTokenEntry.UserId);

            if (user == null)
            {
                resetTokenEntry.AttemptsCount++;
                resetTokenEntry.IsUsed = true;
                await forgotPasswordRepository.UpdateAsync(resetTokenEntry);
                return false;
            }


            user.Credential.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.Credential.LastPasswordChange = DateTime.UtcNow;

            user.Credential.FailedLoginAttempts = 0;
            user.Credential.LockoutUntil = null;

            resetTokenEntry.IsUsed = true;

            await userRepository.SaveUser(user);
            await forgotPasswordRepository.UpdateAsync(resetTokenEntry);

            var alertEmailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #d9534f;'>Security Alert: Password Changed</h2>

                    <p>Dear Customer,</p>

                    <p>The password for your account has been changed successfully.</p>

                    <div style='background: #f8f9fa; border: 1px solid #ddd; padding: 12px; margin: 15px 0; border-radius: 4px;'>
                        <strong>Change Time:</strong> {DateTime.UtcNow}<br/>
                        <strong>IP Address:</strong> {resetTokenEntry.RequestedIp}
                    </div>

                    <p style='color: #c9302c;'>
                        <strong>If you did not make this change,</strong> please contact our Customer Support team immediately and request that your account be temporarily suspended.
                    </p>

                    <p>
                        Thank you,<br/>
                        Security Team
                    </p>
                </div>";

            try
            {
                await emailService.SendEmailAsync(user.Profile.Email, "Reset Your Password", alertEmailBody);

                
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Email send failed: {ex.Message}");

                
            }


            try
            {
                if (!string.IsNullOrEmpty(user.Profile.PhoneNumber))
                {
                    var smsMessage = $"Security Alert: Your account password was changed on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC. If not you, contact bank support immediately.";

                    
                    await smsService.SendSmsAsync(user.Profile.PhoneNumber, smsMessage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SMS send failed: {ex.Message}");
            }

            return true;
        }
    }
}
