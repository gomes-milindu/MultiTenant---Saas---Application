namespace WebApplication1.DTO
{
    public class PackageResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public bool IsFree { get; set; }
        public int? MaxStudents { get; set; }
        public int? MaxClasses { get; set; }
        public int? MaxBatches { get; set; }
        public int? MaxRecordedLessons { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
