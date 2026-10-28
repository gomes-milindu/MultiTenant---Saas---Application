using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface ISubjectRepository
    {
        Task<Subject> CreateAsync(Subject subject);
    }
}
