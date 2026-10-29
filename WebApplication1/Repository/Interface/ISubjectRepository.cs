using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface ISubjectRepository
    {
        Task<Subject> CreateAsync(Subject subject);
        Task<Subject?> GetSubjectByIdAsync(Guid id);
    }
}
