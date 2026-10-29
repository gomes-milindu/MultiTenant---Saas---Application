using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Academic;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Impl
{
    public class ClassRepository:IClassRepository
    {
        private readonly AppDbContext _context;

        public ClassRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsByTitleInBatchAsync(Guid batchId, Guid subjectId, string title)
        {
            var normalizedTitle = title.Trim().ToLower();

            return await _context.Classes
                .AnyAsync(c => c.BatchId == batchId
                            && c.SubjectId == subjectId
                            && c.Title.ToLower() == normalizedTitle);
        }

        public async Task<Class> CreateAsync(Class newclass)
        {
            await _context.Classes.AddAsync(newclass);
            await _context.SaveChangesAsync();
            return newclass;
        }
    }
}
