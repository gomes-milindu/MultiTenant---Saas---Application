using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentDto dto,Guid studentUserId);
        Task<int> GetTotalActiveStudentsCountByTeacherAsync(Guid teacherId);
        Task<bool> ExistsByStudentAndClassAsync(Guid studentUserId, Guid classId);
    }
}
