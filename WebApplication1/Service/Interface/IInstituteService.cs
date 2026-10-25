using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IInstituteService
    {
        Task<InstituteResponseDto> CreateAsync(CreateInstituteDto dto, Guid currentUserId);
    }
}
