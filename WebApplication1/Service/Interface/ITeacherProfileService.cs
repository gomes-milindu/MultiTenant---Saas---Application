using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface ITeacherProfileService
    {
        Task<TeacherProfileResponseDto> CreateOrUpdateMyProfileAsync(TeacherProfileDto dto, Guid currentUserId, bool isAdmin );
        Task<TeacherProfileResponseDto?> GetByTeacherIdAsync(Guid teacherUserId);
    }
}
