using WebApplication1.Data;
using WebApplication1.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Models.Auth;

namespace WebApplication1.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IReadOnlyCollection<UserResponseDto>> GetAllUsers()
        {
            List<UserResponseDto> usersList = await context.Users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Status = u.Status.ToString(),
                Email = u.Profile.Email,
                FirstName = u.Profile.FirstName,
                LastName = u.Profile.LastName,
                PhoneNumber = u.Profile.PhoneNumber
            })
                .ToListAsync();
            return usersList;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await context.Users.Include(u => u.Profile)
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Profile.Email == email);

            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid guid)
        {
            var user = await context.Users.Include(u => u.Profile).Include(u => u.Credential).FirstOrDefaultAsync(u => u.Id == guid);

            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await context.Users.Include(u => u.Profile)
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        public async Task SaveUser(User user)
        {
            var exists = await context.Users.AnyAsync(u => u.Id == user.Id);

            if (exists)
            {
                context.Users.Update(user);
            }
            else
            {
                // Microsoft recommends synchronous Add() over AddAsync() for regular entities
                context.Users.Add(user);
            }

            // EF Core automatically executes this inside an atomic, retriable transaction
            await context.SaveChangesAsync();
        }

        public Task UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
