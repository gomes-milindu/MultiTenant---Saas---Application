using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Impl
{
    public class InstituteRepository : IInstituteRepository
    {

        private readonly AppDbContext _context;

        public InstituteRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<InstituteResponseDto> CreateAsync(Institute institute)
        {
            _context.Institutes.Add(institute);
            await _context.SaveChangesAsync();
            return new InstituteResponseDto
            {
                Id = institute.Id,
                Name = institute.Name,
                Description = institute.Description,
                Status = institute.Status,
                OwnerUserId = institute.OwnerUserId,
                CreatedByUserId = institute.CreatedByUserId,
                CreatedAt = institute.CreatedAt,
                UpdatedAt = institute.UpdatedAt
            };
        }
    }
}
