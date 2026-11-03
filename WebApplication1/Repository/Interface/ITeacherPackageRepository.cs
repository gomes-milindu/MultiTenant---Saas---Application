using WebApplication1.Models.Subscrition;

namespace WebApplication1.Repository.Interface
{
    public interface ITeacherPackageRepository
    {
        Task<TeacherPackage> CreateAsync(TeacherPackage teacherPackage);
        Task<TeacherPackage?> GetActivePackageByUserIdAsync(Guid userId);
    }
}
