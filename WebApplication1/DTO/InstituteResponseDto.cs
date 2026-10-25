using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class InstituteResponseDto
    {

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public InstituteStatus Status { get; set; }
        public Guid OwnerUserId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
