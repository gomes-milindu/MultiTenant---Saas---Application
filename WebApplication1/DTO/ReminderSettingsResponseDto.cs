namespace WebApplication1.DTO
{
    public class ReminderSettingsResponseDto
    {
        public bool IsEnabled { get; set; }
        public bool EnableFirstReminder { get; set; }
        public int FirstReminderDaysBefore { get; set; }
        public bool EnableSecondReminder { get; set; }
        public int SecondReminderDaysBefore { get; set; }
        public bool EnableExpirationEmail { get; set; }
        public string TargetScope { get; set; } = "ALL";
        public List<Guid> SpecificTeacherIds { get; set; } = new();
        public DateTime UpdatedAt { get; set; }
    }
}
