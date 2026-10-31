using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateClassFeePlanDto
    {
        [Required(ErrorMessage = "Class ID is required.")]
        public Guid ClassId { get; set; }

        [Required(ErrorMessage = "Monthly fee is required.")]
        [Range(0, 10000, ErrorMessage = "Monthly fee must be between 0 and 10,000.")]
        public decimal MonthlyFee { get; set; }

        public DateTime? EffectiveFrom { get; set; }
    }
}
