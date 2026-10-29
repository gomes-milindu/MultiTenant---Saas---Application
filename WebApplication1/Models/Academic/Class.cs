using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Academic
{
    [Table("Classes")]
    public class Class
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid InstituteId { get; set; }

        [Required]
        public Guid BatchId { get; set; }

        [Required]
        public Guid SubjectId { get; set; }

        [Required]
        public Guid TeacherUserId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty; // e.g., "Theory & Revision"

        [MaxLength(150)]
        public string Schedule { get; set; } = string.Empty; // e.g., "Sundays 8:00 AM - 12:00 PM"

        [Required]
        public ClassStatus Status { get; set; } = ClassStatus.Active;

        [Required]
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(InstituteId))]
        public Institute Institute { get; set; } = null!;

        [ForeignKey(nameof(BatchId))]
        public Batch Batch { get; set; } = null!;

        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; } = null!;

        [ForeignKey(nameof(TeacherUserId))]
        public User Teacher { get; set; } = null!;

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; } = null!;

    }
}
