namespace WebApplication1.DTOs.Auth.TeacherProfile;

public class TeacherQrScanResponseDto
{
    public Guid TeacherUserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Qualifications { get; set; }
    public string? Bio { get; set; }
    public IEnumerable<TeacherScannedClassDto> ActiveClasses { get; set; } = new List<TeacherScannedClassDto>();
}