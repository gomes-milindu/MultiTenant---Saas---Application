using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateTeacherPackageDto
    {
        [Required]
        public Guid PackageId { get; set; }

        [Required]
        public decimal PricePaid { get; set; }
    }
}
