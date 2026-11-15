using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class UpdateReminderSettingsDto
    {
        public bool IsEnabled { get; set; }

        
        public bool EnableFirstReminder { get; set; }

        [Range(1, 365, ErrorMessage = "First reminder days must be at least 1.")]
        public int FirstReminderDaysBefore { get; set; } = 5;
        public bool EnableSecondReminder { get; set; }

        [Range(1, 365, ErrorMessage = "Second reminder days must be at least 1.")]
        public int SecondReminderDaysBefore { get; set; } = 2;

        public bool EnableExpirationEmail { get; set; }

        [Required]
        [RegularExpression("^(ALL|SPECIFIC)$", ErrorMessage = "TargetScope must be either 'ALL' or 'SPECIFIC'.")]
        public string TargetScope { get; set; } = "ALL";
        public List<Guid>? TeacherIds { get; set; }
    }
}
