using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Subscrition
{
    [Table("PackageReminderSettings")]
    public class PackageReminderSetting
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsEnabled { get; set; } = true;

        public bool EnableFirstReminder { get; set; } = true;
        public int FirstReminderDaysBefore { get; set; } = 5;

        public bool EnableSecondReminder { get; set; } = true;
        public int SecondReminderDaysBefore { get; set; } = 2;

        public bool EnableExpirationEmail { get; set; } = true;

        [Required]
        [MaxLength(20)]
        public string TargetScope { get; set; } = "ALL";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
