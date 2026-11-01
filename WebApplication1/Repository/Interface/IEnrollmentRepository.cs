using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IEnrollmentRepository
    {
        Task<Class?> GetClassForEnrollmentAsync(Guid classId);

        

        Task<bool> HasActiveEnrollmentAsync(Guid studentUserId,Guid classId);

        Task<Enrollment> CreateAsync(Enrollment enrollment);
    }
}
