using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Auth;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Auth.Impl;

public class TeacherProfileRepository : ITeacherProfileRepository
{
    private readonly AppDbContext _context;

    public TeacherProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TeacherProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.TeacherProfiles
            .Include(tp => tp.User)
                .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(tp => tp.UserId == userId);
    }

    public async Task<TeacherProfile> CreateAsync(TeacherProfile profile)
    {
        await _context.TeacherProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
        return profile;
    }

    public async Task UpdateAsync(TeacherProfile profile)
    {
        _context.TeacherProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }
}