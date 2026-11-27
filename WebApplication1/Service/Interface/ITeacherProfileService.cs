using WebApplication1.DTO;
using WebApplication1.DTOs.Auth.TeacherProfile;

namespace WebApplication1.Service.Interface
{
    public interface ITeacherProfileService
    {
        Task<TeacherProfileResponseDto> CreateOrUpdateMyProfileAsync(TeacherProfileDto dto, Guid currentUserId, bool isAdmin );
        Task<TeacherProfileResponseDto?> GetByTeacherIdAsync(Guid teacherUserId);
        Task<TeacherQrScanResponseDto?> ScanTeacherQrAsync(Guid teacherUserId);
    }
}
