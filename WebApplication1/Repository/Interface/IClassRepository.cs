using WebApplication1.DTOs.Auth.TeacherProfile;
using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IClassRepository
    {
        Task<bool> ExistsByTitleInBatchAsync(Guid batchId, Guid subjectId, string title);
        Task<Class> CreateAsync(Class newClass);
        Task<Class> GetByIdAsync(Guid classId);
        Task<int> GetTotalClassesCountByTeacherAsync(Guid? teacherUserId);
        Task<IEnumerable<TeacherScannedClassDto>> GetActiveClassesWithFeeByTeacherIdAsync(Guid teacherUserId);
    }
}
