using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class ClassResponseDto
    {
        public Guid Id { get; set; }
        public Guid InstituteId { get; set; }
        public Guid BatchId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TeacherUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public ClassStatus Status { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
