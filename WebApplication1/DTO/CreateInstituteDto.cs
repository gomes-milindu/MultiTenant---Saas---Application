using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateInstituteDto
    {
        [Required(ErrorMessage = "Institute name is required.")]

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Institute name must be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string? Description { get; set; }

        public string? TeacherUsername { get; set; }
    }
}
