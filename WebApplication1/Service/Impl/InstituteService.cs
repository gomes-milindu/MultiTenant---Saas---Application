using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Models.Academic;
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
            var trimmedName = dto.Name.Trim();

            //var exists = await _instituteRepository.ExistsByNameAsync(trimmedName);
            //if (exists)
            //{
            //    throw new InvalidOperationException("An institute with this name already exists.");
            //}

            Guid ownerId;

            if (!string.IsNullOrWhiteSpace(dto.TeacherUsername))
            {
                
                var teacherUser = await userRepository.GetUserByUsername(dto.TeacherUsername.Trim());
                if (teacherUser == null)
                {
                    throw new InvalidOperationException($"Teacher with username '{dto.TeacherUsername}' does not exist.");
                }
                ownerId = teacherUser.Id;
            }
            else
            {
                ownerId = currentUserId;
            }

            var institute = new Institute
            {
                Id = Guid.NewGuid(),
                Name = trimmedName,
                Description = dto.Description?.Trim(),
                Status = InstituteStatus.Active,
                OwnerUserId = ownerId,             
                CreatedByUserId = currentUserId,   
                CreatedAt = DateTime.UtcNow
            };

            var createdInstitute = await _instituteRepository.CreateAsync(institute);

            return new InstituteResponseDto
            {
                Id = createdInstitute.Id,
                Name = createdInstitute.Name,
                Description = createdInstitute.Description,
                Status = createdInstitute.Status,
                OwnerUserId = createdInstitute.OwnerUserId,
                CreatedByUserId = createdInstitute.CreatedByUserId,
                CreatedAt = createdInstitute.CreatedAt,
                UpdatedAt = createdInstitute.UpdatedAt
            };
        }

        
    }
}
