using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models.Auth;

[Table("TeacherProfiles")]
public class TeacherProfile
{
   
    [Key]
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; } 

    [Required]
    [MaxLength(500)]
    public string QrCodeImageUrl { get; set; } = string.Empty; // Supabase Storage QR PNG URL

    [Required]
    [MaxLength(300)]
    public string QrTargetLink { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public TeachingLevel TeachingLevel { get; set; } = TeachingLevel.AdvancedLevel;

    [MaxLength(200)]
    public string? Qualifications { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}