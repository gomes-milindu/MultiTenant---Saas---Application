using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateBatchDto
    {
        [Required(ErrorMessage = "Institute ID is required.")]
        public Guid InstituteId { get; set; }

        [Required(ErrorMessage = "Batch name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Batch name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Academic year is required.")]
        [Range(2000, 2100, ErrorMessage = "Academic year must be between 2000 and 2100.")]
        public int AcademicYear { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
