using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IBatchRepository
    {
        Task<bool> batchExcistforInstitute(string trimmedName, Guid instituteId);
        Task<Batch> CreateAsync(Batch batch);

        Task<Batch?> GetByIdAsync(Guid id);

        Task<IEnumerable<Batch>> GetAllAsync();
    }
}
