using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateEnrollmentDto
    {
        [Required(ErrorMessage = "Class ID is required.")]
        public Guid ClassId { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
