namespace WebApplication1.DTOs.Auth.TeacherProfile;

public class TeacherScannedClassDto
{
    public Guid ClassId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Schedule { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string BatchName { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public string InstituteName { get; set; } = string.Empty;
    public decimal? MonthlyFee { get; set; }
}