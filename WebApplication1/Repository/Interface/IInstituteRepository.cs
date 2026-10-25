using WebApplication1.DTO;
using WebApplication1.Models.Academic;

namespace WebApplication1.Repository.Interface
{
    public interface IInstituteRepository
    {
        Task<InstituteResponseDto> CreateAsync(Institute institute);
        Task<bool> ExistsByNameAsync(string trimmedName, Guid currentUserId);
    }
}
