using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Subscriptions;

namespace WebApplication1.Models.Subscrition;

    [Table("TeacherPackages")]
public class TeacherPackage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid PackageId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePaid { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool FirstReminderSent { get; set; } = false;

    public bool SecondReminderSent { get; set; } = false;

    public bool ExpirationEmailSent { get; set; } = false;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(PackageId))]
    public Package Package { get; set; } = null!;
}

