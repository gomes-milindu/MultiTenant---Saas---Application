namespace WebApplication1.DTO
{
    public class TeacherPackageResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ActivatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public decimal PricePaid { get; set; }
    }
}
