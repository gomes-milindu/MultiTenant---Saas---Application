using WebApplication1.Data;
using WebApplication1.Models.Subscrition;
using WebApplication1.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Repository.Impl
{
    public class TeacherPackageRepository:ITeacherPackageRepository
    {
        private readonly AppDbContext _context;

        public TeacherPackageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherPackage> CreateAsync(TeacherPackage teacherPackage)
        {
            await _context.TeacherPackages.AddAsync(teacherPackage);
            await _context.SaveChangesAsync();
            return teacherPackage;
        }

        public async Task<TeacherPackage?> GetActivePackageByUserIdAsync(Guid userId)
        {

            return await _context.TeacherPackages
                .Include(tp => tp.Package)
                .FirstOrDefaultAsync(tp => tp.UserId == userId
                                        && tp.Status == "ACTIVE"
                                        && tp.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<TeacherPackage?> GetByIdAsync(Guid userId)
        {
            return await _context.TeacherPackages
        .Include(tp => tp.Package)
        .FirstOrDefaultAsync(tp => tp.UserId == userId);
        }
        public async Task UpdateAsync(TeacherPackage teacherPackage)
        {
            _context.TeacherPackages.Update(teacherPackage);
            await _context.SaveChangesAsync();
        }
    }
}
