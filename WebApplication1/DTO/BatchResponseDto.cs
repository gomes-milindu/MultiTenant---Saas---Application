using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class BatchResponseDto
    {
        public Guid Id { get; set; }
        public Guid InstituteId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AcademicYear { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public BatchStatus Status { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
