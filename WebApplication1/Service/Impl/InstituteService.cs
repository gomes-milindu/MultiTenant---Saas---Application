using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Auth;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class InstituteService : IInstituteService
    {
        private readonly IInstituteRepository _instituteRepository;
        private readonly IUserRepository userRepository;

        public InstituteService(IInstituteRepository instituteRepository, IUserRepository userRepository)
        {
            _instituteRepository = instituteRepository;
            this.userRepository = userRepository;
        }

        public async Task<InstituteResponseDto> CreateAsync(CreateInstituteDto dto, Guid currentUserId)
        {
            var name = dto.Name.Trim();
            var hasTeacher = !string.IsNullOrWhiteSpace(dto.TeacherUsername);

            var creator = await userRepository.GetUserByIdAsync(currentUserId)
                ?? throw new InvalidOperationException("Current user not found.");

            var isSuperAdmin = creator.Role == UserRole.SuperAdmin;
            var isTeacher = creator.Role == UserRole.Teacher;

            if (!isSuperAdmin && !isTeacher)
                throw new InvalidOperationException("You are not allowed to create institutes.");

            
            if (isSuperAdmin && !hasTeacher)
                throw new InvalidOperationException("Super admin must provide a teacher username.");

            User owner = creator; // default
            

            if (hasTeacher)
            {
                owner = await userRepository.GetUserByUsername(dto.TeacherUsername!.Trim())
                    ?? throw new InvalidOperationException($"Teacher '{dto.TeacherUsername}' does not exist.");

                // teacher 2 logged and pass teacher -1 user name, blocking
                if (isTeacher && owner.Id != creator.Id)
                    throw new InvalidOperationException("Teachers can only create institutes for themselves.");

                
                if (owner.Role != UserRole.Teacher)
                    throw new InvalidOperationException("The specified user is not a teacher.");
            }

            if (await _instituteRepository.ExistsByNameAsync(name, owner.Id))
                throw new InvalidOperationException("This teacher already has an institute with this name.");

            var institute = new Institute
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = dto.Description?.Trim(),
                Status = InstituteStatus.Active,
                OwnerUserId = owner.Id,
                CreatedByUserId = creator.Id,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _instituteRepository.CreateAsync(institute);
            return new InstituteResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Status = created.Status,
                OwnerUserId = created.OwnerUserId,
                CreatedByUserId = created.CreatedByUserId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            };
        }


    }
}
