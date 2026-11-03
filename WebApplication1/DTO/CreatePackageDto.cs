using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreatePackageDto
    {
        [Required(ErrorMessage = "Package code is required.")]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Package name is required.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 10000, ErrorMessage = "Price must be a valid amount.")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = "LKR";

        [Required]
        [Range(1, 3650, ErrorMessage = "Duration must be at least 1 day.")]
        public int DurationDays { get; set; }

        public bool IsFree { get; set; } = false;

        // NULL = Unlimited
        public int? MaxStudents { get; set; }
        public int? MaxClasses { get; set; }
        public int? MaxBatches { get; set; }
        public int? MaxRecordedLessons { get; set; }

        public int SortOrder { get; set; }
    }
}
