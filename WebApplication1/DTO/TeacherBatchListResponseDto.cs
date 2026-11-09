using WebApplication1.DTO;

namespace WebApplication1.DTOs.Academic.Batch;

public class TeacherBatchListResponseDto
{
    public int TotalCount { get; set; }
    public IEnumerable<BatchResponseDto> Batches { get; set; } = Enumerable.Empty<BatchResponseDto>();
}