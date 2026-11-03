using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IPackageService
    {
        Task<PackageResponseDto> CreateAsync(CreatePackageDto dto, Guid currentUserId);
        Task<PackageResponseDto> GetPackagesByIdAsync(Guid id);
    }
}
