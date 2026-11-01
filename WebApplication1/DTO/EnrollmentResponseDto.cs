using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class EnrollmentResponseDto
    {
        public Guid Id { get; set; }

        public Guid StudentUserId { get; set; }

        public Guid ClassId { get; set; }

        public Guid BatchId { get; set; }

        public EnrollmentStatus Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public Guid? ApprovedBy { get; set; }

        public decimal? AgreedMonthlyFee { get; set; }

        public string? Notes { get; set; }
    }
}
