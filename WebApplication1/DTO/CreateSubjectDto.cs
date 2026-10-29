using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateSubjectDto
    {
        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Subject name must be between 2 and 40 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        public string? Description { get; set; }
        public Guid? OwnerTeacherId { get; set; }
    }
}
