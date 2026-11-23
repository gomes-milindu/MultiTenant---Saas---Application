using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class TeacherProfileDto
    {
        [MaxLength(200)]
        public string? TeacherId { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [MaxLength(100)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teaching level is required.")]
        public TeachingLevel TeachingLevel { get; set; } = TeachingLevel.AdvancedLevel;

        [MaxLength(200)]
        public string? Qualifications { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }
}
