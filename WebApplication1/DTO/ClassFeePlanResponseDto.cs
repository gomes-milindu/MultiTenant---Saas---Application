namespace WebApplication1.DTOs.Academic.ClassFeePlan;

public class ClassFeePlanResponseDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassTitle { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}