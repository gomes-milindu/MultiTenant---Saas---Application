using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Academic;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Impl
{
    public class ClassFeePlanRepository:IClassFeePlanRepository
    {
        private readonly AppDbContext _context;

        public ClassFeePlanRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ClassFeePlan?> GetActivePlanByClassIdAsync(Guid classId)
        {
            
            return await _context.ClassFeePlans
                .FirstOrDefaultAsync(fp => fp.ClassId == classId && fp.IsActive);
        }


        public async Task UpdateAsync(ClassFeePlan feePlan)
        {
            _context.ClassFeePlans.Update(feePlan);
            await _context.SaveChangesAsync();
        }

        public async Task<ClassFeePlan> CreateAsync(ClassFeePlan feePlan)
        {
            await _context.ClassFeePlans.AddAsync(feePlan);
            await _context.SaveChangesAsync();
            return feePlan;
        }

        public async Task<ClassFeePlan?> GetByIdAsync(Guid id)
        {
            return await _context.ClassFeePlans
                .Include(fp => fp.Class)
                .Include(fp => fp.CreatedByUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(fp => fp.Id == id);
        }

        public async Task<ClassFeePlan?> GetCurrentFeePlanAsync(Guid classId,DateTime currentDate)
        {
            return await _context.ClassFeePlans
                .AsNoTracking()
                .Where(f => f.ClassId == classId
                    && f.IsActive
                    && f.EffectiveFrom <= currentDate
                    && (f.EffectiveTo == null || f.EffectiveTo >= currentDate))
                .OrderByDescending(f => f.EffectiveFrom)
                .FirstOrDefaultAsync();
        }
    }
}
