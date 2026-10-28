using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;
// Add your User model namespace, e.g.:
// using WebApplication1.Models; 

namespace WebApplication1.Models.Academic;

[Table("Institutes")]
public class Institute
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public InstituteStatus Status { get; set; } = InstituteStatus.Active;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid OwnerUserId { get; set; }

    [Required]
    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(OwnerUserId))]
    public User? OwnerUser { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public User? CreatedByUser { get; set; }

}