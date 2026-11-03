using WebApplication1.Models.Subscriptions;

namespace WebApplication1.Repository.Interface
{
    public interface IPackageRepository
    {
        Task<Package> CreateAsync(Package package);
        Task<bool> ExistsByCodeAsync(string trimmedCode);
        Task<Package?> GetPackagesByIdAsync(Guid id);
    }
}
