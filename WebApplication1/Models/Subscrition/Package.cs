using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Subscriptions;

[Table("Packages")] // 🟢 අකුරු වල සංවේදීතාව (Case-Sensitivity) සඳහා "Packages" ලෙස ලබා දෙන්න
public class Package
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // Unique Code (උදා: "FREE_TRIAL")

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "LKR";

    [Required]
    public int DurationDays { get; set; } // පැකේජයේ කාලසීමාව (උදා: 30)

    public bool IsFree { get; set; } = false;

    // Usage Limits (NULL = Unlimited)
    public int? MaxStudents { get; set; }
    public int? MaxClasses { get; set; }
    public int? MaxBatches { get; set; }
    public int? MaxRecordedLessons { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } // පැකේජ පෙන්වන අනුපිළිවෙළ

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}