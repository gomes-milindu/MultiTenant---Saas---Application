using QRCoder;
using WebApplication1.DTO;

using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Auth;
using WebApplication1.Repository.Auth.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;
using WebApplication1.Service.Storage;

namespace WebApplication1.Service.Auth.Impl;

public class TeacherProfileService : ITeacherProfileService
{
    private readonly ITeacherProfileRepository teacherProfileRepository;
    private readonly IUserRepository userRepository;
    private readonly IFileStorageService fileStorageService;
    private readonly IConfiguration _configuration;

    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

    public TeacherProfileService(ITeacherProfileRepository teacherProfileRepository, IUserRepository userRepository, IFileStorageService fileStorageService, IConfiguration configuration)
    {
        this.teacherProfileRepository = teacherProfileRepository;
        this.userRepository = userRepository;
        this.fileStorageService = fileStorageService;
        _configuration = configuration;
    }

    public async Task<TeacherProfileResponseDto> CreateOrUpdateMyProfileAsync(TeacherProfileDto dto, Guid currentUserId, bool isAdmin)
    {
        var teachersId = Guid.TryParse(dto.TeacherId, out Guid teacherId);

        if (isAdmin && teachersId && teacherId == Guid.Empty)
        {
            throw new ArgumentException("Teacher ID is required for admin requests.", nameof(dto.TeacherId));
        }

        Guid targetUserId = isAdmin ? teacherId : currentUserId;
        
        var user = await userRepository.GetUserByIdAsync(targetUserId);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (user.Status != AccountStatus.Active)
            throw new InvalidOperationException("Your account is inactive or suspended.");


        if (!isAdmin && user.Id != currentUserId)
            throw new UnauthorizedAccessException("Only each teacher can generate their own qr");


        // 2. Profile Picture Validation
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            if (dto.ProfilePicture.Length > MaxImageSizeBytes)
                throw new InvalidOperationException("Profile picture size cannot exceed 2 MB.");

            var ext = Path.GetExtension(dto.ProfilePicture.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
                throw new InvalidOperationException("Only .jpg, .jpeg, .png, and .webp images are allowed.");
        }

        var existingProfile = await teacherProfileRepository.GetByUserIdAsync(targetUserId);

        if (existingProfile == null)
        {
            var baseUrl = _configuration["Frontend:BaseUrl"]?.TrimEnd('/') ?? "https://localhost:7001/api/TeacherProfiles/scan";
            var qrTargetLink = $"{baseUrl}/{currentUserId}";

            var qrPngBytes = GenerateQrCodePng(qrTargetLink);
            var qrImageUrl = await fileStorageService.UploadBytesAsync(
                qrPngBytes,
                "teacher-qrs",
                $"qr-{currentUserId}.png",
                "image/png"
            );

            string? profilePicUrl = null;
            if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
            {
                profilePicUrl = await fileStorageService.UploadFileAsync(dto.ProfilePicture, "teacher-avatars");
            }

            var newProfile = new TeacherProfile
            {
                UserId = targetUserId,
                Subject = dto.Subject.Trim(),
                TeachingLevel = dto.TeachingLevel,
                Qualifications = dto.Qualifications?.Trim(),
                Bio = dto.Bio?.Trim(),
                ProfilePictureUrl = profilePicUrl,
                QrTargetLink = qrTargetLink,
                QrCodeImageUrl = qrImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await teacherProfileRepository.CreateAsync(newProfile);
            var saved = await teacherProfileRepository.GetByUserIdAsync(targetUserId);
            return MapToDto(saved!);
        }
        else
        {
            existingProfile.Subject = dto.Subject.Trim();
            existingProfile.TeachingLevel = dto.TeachingLevel;
            existingProfile.Qualifications = dto.Qualifications?.Trim();
            existingProfile.Bio = dto.Bio?.Trim();
            existingProfile.UpdatedAt = DateTime.UtcNow;

            if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
            {
                await fileStorageService.DeleteFileByUrlAsync(existingProfile.ProfilePictureUrl);
                existingProfile.ProfilePictureUrl = await fileStorageService.UploadFileAsync(dto.ProfilePicture, "teacher-avatars");
            }

            await teacherProfileRepository.UpdateAsync(existingProfile);
            return MapToDto(existingProfile);
        }
    }


    private static byte[] GenerateQrCodePng(string targetUrl)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(targetUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }



    public async Task<TeacherProfileResponseDto?> GetByTeacherIdAsync(Guid teacherUserId)
    {
        var profile = await teacherProfileRepository.GetByUserIdAsync(teacherUserId);
        if (profile == null)
        {
            return null;
        }

        var fullName = profile.User?.Profile != null
            ? $"{profile.User.Profile.FirstName} {profile.User.Profile.LastName}".Trim()
            : null;

        return MapToDto(profile);
    }



    private static TeacherProfileResponseDto MapToDto(TeacherProfile tp)
    {
        var fullName = tp.User?.Profile != null ? $"{tp.User.Profile.FirstName} {tp.User.Profile.LastName}".Trim() : null;

        return new TeacherProfileResponseDto
        {
            UserId = tp.UserId,
            TeacherUsername = tp.User?.Username ?? string.Empty,
            FullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName,
            ProfilePictureUrl = tp.ProfilePictureUrl,
            QrCodeImageUrl = tp.QrCodeImageUrl,
            QrTargetLink = tp.QrTargetLink,
            Subject = tp.Subject,
            TeachingLevel = tp.TeachingLevel,
            TeachingLevelDisplay = tp.TeachingLevel.ToString(),
            Qualifications = tp.Qualifications,
            Bio = tp.Bio,
            CreatedAt = tp.CreatedAt,
            UpdatedAt = tp.UpdatedAt
        };
    }
}