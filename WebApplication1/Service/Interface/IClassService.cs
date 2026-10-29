using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IClassService
    {
        Task<ClassResponseDto> CreateAsync(CreateClassDto dto, Guid currentUserId, bool isAdmin);
    }
}
