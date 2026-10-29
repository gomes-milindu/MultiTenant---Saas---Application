using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Academic;
using WebApplication1.Repository.Interface;


namespace WebApplication1.Repository.Impl
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<Subject?> GetSubjectByIdAsync(Guid id)
        {
            return await _context.Subjects
                .Include(s => s.OwnerUser)       // අදාළ Teacher විස්තර අවශ්‍ය නම්
                .Include(s => s.CreatedByUser)   // හැදූ User විස්තර අවශ්‍ය නම්
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        
    }
    
}
