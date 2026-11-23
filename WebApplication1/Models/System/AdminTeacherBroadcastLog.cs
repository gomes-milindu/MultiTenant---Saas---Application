using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;

namespace WebApplication1.Models.System
{
    [Table("SuperAdminMsg")]
    public class AdminTeacherBroadcastLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SentBySuperAdminId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Channel { get; set; } = "EMAIL"; // "EMAIL", "SMS", or "BOTH"

        [Required]
        [MaxLength(20)]
        public string TargetScope { get; set; } = "ALL"; // "ALL" or "SPECIFIC"

        [MaxLength(200)]
        public string? Subject { get; set; }

        [Required]
        public string MessageBody { get; set; } = string.Empty;

        public int TotalEmailsSent { get; set; }
        public int TotalSmsSent { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SentBySuperAdminId))]
        public User SentBySuperAdmin { get; set; } = null!;
    }
}
