using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class TeacherProfileResponseDto
    {
        public Guid UserId { get; set; }
        public string TeacherUsername { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string QrCodeImageUrl { get; set; } = string.Empty;
        public string QrTargetLink { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public TeachingLevel TeachingLevel { get; set; }
        public string TeachingLevelDisplay { get; set; } = string.Empty;
        public string? Qualifications { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
