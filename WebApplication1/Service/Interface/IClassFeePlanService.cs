using WebApplication1.DTO;
using WebApplication1.DTOs.Academic.ClassFeePlan;

namespace WebApplication1.Service.Interface
{
    public interface IClassFeePlanService
    {
        Task<ClassFeePlanResponseDto> CreateAsync(CreateClassFeePlanDto dto, Guid currentUserId, bool isAdmin);
    }
}
