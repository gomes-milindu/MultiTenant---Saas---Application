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

        public async Task<PackageReminderSetting?> GetSettingsAsync()
        {
            return await _context.PackageReminderSettings.FirstOrDefaultAsync();
        }

        public async Task<PackageReminderSetting> CreateSettingsAsync(PackageReminderSetting settings)
        {
            await _context.PackageReminderSettings.AddAsync(settings);
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<List<Guid>> GetTargetTeacherIdsAsync()
        {
            return await _context.PackageReminderTargetTeachers
                .AsNoTracking()
                .Select(t => t.TeacherUserId)
                .ToListAsync();
        }


        public async Task UpdateSettingsAsync(PackageReminderSetting settings)
        {
            _context.PackageReminderSettings.Update(settings);
            await _context.SaveChangesAsync();
        }

        public async Task ReplaceTargetTeachersAsync(IEnumerable<Guid> teacherIds)
        {
            var existingTargets = await _context.PackageReminderTargetTeachers.ToListAsync();
            _context.PackageReminderTargetTeachers.RemoveRange(existingTargets);

            var newTargets = teacherIds.Distinct().Select(id => new PackageReminderTargetTeacher
            {
                Id = Guid.NewGuid(),
                TeacherUserId = id
            });

            await _context.PackageReminderTargetTeachers.AddRangeAsync(newTargets);
            await _context.SaveChangesAsync();
        }
    }
}
