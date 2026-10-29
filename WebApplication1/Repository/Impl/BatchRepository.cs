using WebApplication1.Data;
using WebApplication1.Models.Academic;
using WebApplication1.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Repository.Impl
{
    public class BatchRepository : IBatchRepository
    {
        private readonly AppDbContext _context;

        public BatchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> batchExcistforInstitute(string trimmedName, Guid instituteId)
        {
            return await _context.Batches.AnyAsync(b => b.InstituteId == instituteId
                    && b.Name.ToLower() == trimmedName.ToLower());
        }

        public async Task<Batch> CreateAsync(Batch batch)
        {
            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<Batch?> GetByIdAsync(Guid id)
        {
            return await _context.Batches
                .Include(b => b.Institute)
                
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Batch>> GetAllAsync()
        {
            return await _context.Batches
                .Include(b => b.Institute)
                .Include(b => b.OwnerTeacher)
                .IgnoreQueryFilters()
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
