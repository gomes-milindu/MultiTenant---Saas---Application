using WebApplication1.DTO;

using WebApplication1.Service.Interface;
using WebApplication1.Repository.Interface;
using WebApplication1.Models.System;
using WebApplication1.Models.Enums;


namespace WebApplication1.Service.Communication.Impl;

public class AdminMessagingService : IAdminMessagingService
{
    private readonly IAdminMessagingRepository adminMessagingRepository;
    private readonly IEmailService emailService;
    private readonly ISmsService smsService;

    public AdminMessagingService(IAdminMessagingRepository adminMessagingRepository, IEmailService emailService, ISmsService smsService)
    {
        this.adminMessagingRepository = adminMessagingRepository;
        this.emailService = emailService;
        this.smsService = smsService;
    }

    public async Task<object> BroadcastToTeachersAsync(SendAdminTeacherMessageDto dto, Guid superAdminId)
    {
        if ((dto.Channel == MessageChannel.EMAIL || dto.Channel == MessageChannel.BOTH) && string.IsNullOrWhiteSpace(dto.Subject))
        {
            throw new InvalidOperationException("Subject is required when sending an Email.");
        }

        if (dto.TargetScope == TargetScopeType.SPECIFIC && (dto.TeacherIds == null || !dto.TeacherIds.Any()))
        {
            throw new InvalidOperationException("Please provide at least one TeacherId when TargetScope is SPECIFIC.");
        }

        // All -> null ; Specifiv -> Teacher Id
        var targetIds = dto.TargetScope == TargetScopeType.SPECIFIC ? dto.TeacherIds : null;
        var teachers = await adminMessagingRepository.GetActiveTeachersAsync(targetIds);

        if (!teachers.Any())
        {
            throw new KeyNotFoundException("No active teachers found for the selected criteria.");
        }

        var targetEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var targetPhones = new HashSet<string>();

        foreach (var teacher in teachers)
        {
            if (teacher.Profile == null) continue;

            if (!string.IsNullOrWhiteSpace(teacher.Profile.Email))
                targetEmails.Add(teacher.Profile.Email.Trim());

            if (!string.IsNullOrWhiteSpace(teacher.Profile.PhoneNumber))
                targetPhones.Add(teacher.Profile.PhoneNumber.Trim());
        }

        int emailsSent = 0;
        int smsSent = 0;

        
        if (dto.Channel == MessageChannel.EMAIL || dto.Channel == MessageChannel.BOTH)
        {
            foreach (var email in targetEmails)
            {
                await emailService.SendEmailAsync(email, dto.Subject!, dto.MessageBody);
                emailsSent++;
            }
        }

        
        if (dto.Channel == MessageChannel.SMS || dto.Channel == MessageChannel.BOTH)
        {
            foreach (var phone in targetPhones)
            {
                await smsService.SendSmsAsync(phone, dto.MessageBody);
                smsSent++;
            }
        }

        
        var log = new AdminTeacherBroadcastLog
        {
            SentBySuperAdminId = superAdminId,
            Channel = dto.Channel.ToString(),
            TargetScope = dto.TargetScope.ToString(),
            Subject = dto.Subject,
            MessageBody = dto.MessageBody,
            TotalEmailsSent = emailsSent,
            TotalSmsSent = smsSent,
            SentAt = DateTime.UtcNow
        };

        await adminMessagingRepository.CreateBroadcastLogAsync(log);

        return new
        {
            Success = true,
            Message = "Message dispatched to teachers successfully.",
            TotalTeachersMatched = teachers.Count,
            TotalEmailsSent = emailsSent,
            TotalSmsSent = smsSent
        };
    }
}