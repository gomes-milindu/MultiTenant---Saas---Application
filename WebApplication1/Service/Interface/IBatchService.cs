using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IBatchService
    {
        Task<BatchResponseDto> CreateAsync(CreateBatchDto dto, Guid currentUserId, bool isAdmin);
    }
}
