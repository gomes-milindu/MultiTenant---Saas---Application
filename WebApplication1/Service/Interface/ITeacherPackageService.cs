using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface ITeacherPackageService
    {
        Task<TeacherPackageResponseDto> SubscribeAsync(CreateTeacherPackageDto dto, Guid currentUserId);
    }
}
