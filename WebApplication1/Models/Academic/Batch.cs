using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Academic
{
    [Table("Batches")]
    public class Batch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid InstituteId { get; set; }

        [Required]
        public Guid OwnerTeacherId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int AcademicYear { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public BatchStatus Status { get; set; } = BatchStatus.Active;

        [Required]
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(InstituteId))]
        public Institute Institute { get; set; } = null!;

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = null!;

        [ForeignKey(nameof(OwnerTeacherId))]
        public User OwnerTeacher { get; set; } = null!;
    }
}
