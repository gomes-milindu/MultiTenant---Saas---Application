using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface ISubjectService
    {
        Task<SubjectResponseDto> CreateAsync(CreateSubjectDto dto, Guid currentUserId, bool isAdmin);
    }
}
