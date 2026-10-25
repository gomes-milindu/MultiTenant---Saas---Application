using WebApplication1.DTO;
using WebApplication1.Models.Auth;

namespace WebApplication1.Repository.Interface
{
    public interface IUserRepository
    {
        Task<IReadOnlyCollection<UserResponseDto>> GetAllUsers();
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByIdAsync(Guid guid);
        Task<User> GetUserByUsername(string username);
        Task SaveUser(User user);
        
        Task UpdateUser(User user);
    }
}
