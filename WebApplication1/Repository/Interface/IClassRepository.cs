using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IClassRepository
    {
        Task<bool> ExistsByTitleInBatchAsync(Guid batchId, Guid subjectId, string title);
        Task<Class> CreateAsync(Class newClass);
        Task<Class> GetByIdAsync(Guid classId);
    }
}
