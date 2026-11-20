using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IAdminMessagingService
    {
        Task<object> BroadcastToTeachersAsync(SendAdminTeacherMessageDto dto, Guid superAdminId);
    }
}
