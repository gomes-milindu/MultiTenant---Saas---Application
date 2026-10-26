using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IBatchRepository
    {
        Task<Batch> CreateAsync(Batch batch);
    }
}
