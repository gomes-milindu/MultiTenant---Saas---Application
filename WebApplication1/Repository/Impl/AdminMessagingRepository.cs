using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Auth;

using WebApplication1.Models.Enums;
using WebApplication1.Models.System;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Communication.Impl;

public class AdminMessagingRepository : IAdminMessagingRepository
{
    private readonly AppDbContext _context;

    public AdminMessagingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetActiveTeachersAsync(List<Guid>? specificTeacherIds = null)
    {
        var query = _context.Users
            .Include(u => u.Profile)
            .Where(u => u.Role == UserRole.Teacher && u.Status == AccountStatus.Active)
            .AsNoTracking();

        if (specificTeacherIds != null && specificTeacherIds.Any())
        {
            var distinctIds = specificTeacherIds.Distinct().ToList();
            query = query.Where(u => distinctIds.Contains(u.Id));
        }

        return await query.ToListAsync();
    }

    public async Task<AdminTeacherBroadcastLog> CreateBroadcastLogAsync(AdminTeacherBroadcastLog log)
    {
        await _context.AdminTeacherBroadcastLogs.AddAsync(log);
        await _context.SaveChangesAsync();
        return log;
    }
}