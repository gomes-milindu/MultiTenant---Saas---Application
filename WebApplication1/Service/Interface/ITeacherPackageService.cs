using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface ITeacherPackageService
    {
        Task<bool> ActivatePackageAsync(Guid teacherPackageId);
        Task<TeacherPackageResponseDto> GetTeacherPackagesByIdAsync(Guid id, Guid currentUserId);
        Task<TeacherPackageResponseDto> SubscribeAsync(CreateTeacherPackageDto dto, Guid currentUserId);
        Task<RemainingDaysDto> GetRemainingDaysAsync(Guid teacherId,Guid currentUserId, bool isAdmin);
    }
}
