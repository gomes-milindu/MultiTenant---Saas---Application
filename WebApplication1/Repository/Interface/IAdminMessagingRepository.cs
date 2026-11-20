using WebApplication1.Models.Auth;
using WebApplication1.Models.System;

namespace WebApplication1.Repository.Interface
{
    public interface IAdminMessagingRepository
    {
        Task<List<User>> GetActiveTeachersAsync(List<Guid>? specificTeacherIds = null);
        Task<AdminTeacherBroadcastLog> CreateBroadcastLogAsync(AdminTeacherBroadcastLog log);
    }
}
