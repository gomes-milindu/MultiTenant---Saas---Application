using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Academic
{
    [Table("Enrollments")]
    public class Enrollment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid StudentUserId { get; set; }

        [Required]
        public Guid ClassId { get; set; }

        [Required]
        public Guid BatchId { get; set; }

        [Required]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        public Guid? ApprovedBy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AgreedMonthlyFee { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation properties

        [ForeignKey(nameof(StudentUserId))]
        public User Student { get; set; } = null!;

        [ForeignKey(nameof(ClassId))]
        public Class Class { get; set; } = null!;

        [ForeignKey(nameof(BatchId))]
        public Batch Batch { get; set; } = null!;

        [ForeignKey(nameof(ApprovedBy))]
        public User? Approver { get; set; }
    }
}
