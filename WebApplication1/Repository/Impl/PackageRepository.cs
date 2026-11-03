using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Subscriptions;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Impl
{
    public class PackageRepository : IPackageRepository
    {
        private readonly AppDbContext _context;
        public PackageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsByCodeAsync(string trimmedCode)
        {
            return await _context.Packages.AnyAsync(p => p.Code.ToUpper() == trimmedCode);
        }

        public async Task<Package> CreateAsync(Package package)
        {
            await _context.Packages.AddAsync(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<Package?> GetPackagesByIdAsync(Guid id)
        {
            return await _context.Packages
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
    
}
