using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IClassFeePlanRepository
    {
        

        Task<ClassFeePlan?> GetActivePlanByClassIdAsync(Guid classId);

        Task<ClassFeePlan?> GetByIdAsync(Guid id);
        Task<ClassFeePlan> CreateAsync(ClassFeePlan feePlan);
        Task UpdateAsync(ClassFeePlan feePlan);
    }
}
