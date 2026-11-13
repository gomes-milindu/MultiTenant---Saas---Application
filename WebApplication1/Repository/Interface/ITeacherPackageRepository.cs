using WebApplication1.Models.Subscrition;

namespace WebApplication1.Repository.Interface
{
    public interface ITeacherPackageRepository
    {
        Task<TeacherPackage> CreateAsync(TeacherPackage teacherPackage);
        Task<TeacherPackage?> GetActivePackageByUserIdAsync(Guid userId);
        Task<TeacherPackage?> GetByIdAsync(Guid teacherPackageId);
        Task UpdateAsync(TeacherPackage teacherPackage);
        Task<PackageReminderSetting?> GetSettingsAsync();
    }
}
