using WebApplication1.DTO;
using WebApplication1.DTOs.Academic.Batch;
using WebApplication1.Models.Academic;

namespace WebApplication1.Service.Interface
{
    public interface IBatchService
    {
        Task<BatchResponseDto> CreateAsync(CreateBatchDto dto, Guid currentUserId, bool isAdmin);
        Task<IEnumerable<BatchResponseDto>> GetAllAsync();
        Task<TeacherBatchListResponseDto> GetBatchesByTeacherAsync(Guid teacherId , Guid currentUserId, bool isAdmin);
    }
}
