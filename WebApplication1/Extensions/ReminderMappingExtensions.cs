using WebApplication1.DTO;
using WebApplication1.Models.Subscrition;


namespace WebApplication1.Extensions
{
    public static class ReminderMappingExtensions
    {
        public static ReminderSettingsResponseDto ToDto(this PackageReminderSetting settings, List<Guid> specificTeacherIds)
        {
            return new ReminderSettingsResponseDto
            {
                IsEnabled = settings.IsEnabled,
                EnableFirstReminder = settings.EnableFirstReminder,
                FirstReminderDaysBefore = settings.FirstReminderDaysBefore,
                EnableSecondReminder = settings.EnableSecondReminder,
                SecondReminderDaysBefore = settings.SecondReminderDaysBefore,
                EnableExpirationEmail = settings.EnableExpirationEmail,
                TargetScope = settings.TargetScope,
                SpecificTeacherIds = specificTeacherIds, 
                UpdatedAt = settings.UpdatedAt
            };
        }
    }
}
