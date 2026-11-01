using WebApplication1.Data;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Impl
{
    public class EnrollmentRepository:IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Class?> GetClassForEnrollmentAsync(Guid classId)
        {
            return await _context.Classes
                .Include(c => c.Batch)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == classId);
        }

        

        public async Task<bool> HasActiveEnrollmentAsync(Guid studentUserId,Guid classId)
        {
            return await _context.Enrollments
                .AnyAsync(e =>
                    e.StudentUserId == studentUserId &&
                    e.ClassId == classId &&
                    (e.Status == EnrollmentStatus.Pending ||
                     e.Status == EnrollmentStatus.Approved));
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);

            await _context.SaveChangesAsync();

            return enrollment;
        }
    }
}
