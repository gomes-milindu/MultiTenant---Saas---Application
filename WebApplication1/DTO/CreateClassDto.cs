using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.DTO
{
    public class CreateClassDto
    {
        [JsonRequired]
        [Required(ErrorMessage = "Institute ID is required.")]
        public Guid InstituteId { get; set; }

        [JsonRequired]
        [Required(ErrorMessage = "Batch ID is required.")]
        public Guid BatchId { get; set; }

        [JsonRequired]
        [Required(ErrorMessage = "Subject ID is required.")]
        public Guid SubjectId { get; set; }
        public Guid? TeacherUserId { get; set; }

        [Required(ErrorMessage = "Class title is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Class title must be between 2 and 150 characters.")]
        public string Title { get; set; } = string.Empty; 

        [MaxLength(150, ErrorMessage = "Schedule cannot exceed 150 characters.")]
        public string Schedule { get; set; } = string.Empty;
    }
}
