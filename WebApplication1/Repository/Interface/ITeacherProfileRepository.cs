using WebApplication1.Models.Auth;

namespace WebApplication1.Repository.Interface
{
    public interface ITeacherProfileRepository
    {
        Task<TeacherProfile?> GetByUserIdAsync(Guid userId);
        Task<TeacherProfile> CreateAsync(TeacherProfile profile);
        Task UpdateAsync(TeacherProfile profile);
    }
}
