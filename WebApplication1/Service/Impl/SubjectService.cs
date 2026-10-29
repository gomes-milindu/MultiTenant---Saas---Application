using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;
namespace WebApplication1.Service.Impl
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository subjectRepository;
        private readonly IUserRepository UserRepository;

        public SubjectService(ISubjectRepository subjectRepository, IUserRepository UserRepository)
        {
            this.subjectRepository = subjectRepository;
            this.UserRepository = UserRepository;
        }

        public async Task<SubjectResponseDto> CreateAsync(CreateSubjectDto dto, Guid currentUserId, bool isAdmin)
        {
            var trimmedName = dto.Name.Trim();
            Guid finalOwnerId;

            var ownerTeacher = await UserRepository.GetUserByIdAsync(dto.OwnerTeacherId.Value);

            if (isAdmin)
            {
                

                if (ownerTeacher is null)
                    throw new ArgumentException($"Teacher '{ownerTeacher.Username}' was not found.");

                finalOwnerId = ownerTeacher.Id;
            }
            else
            {
                finalOwnerId = currentUserId;
            }

            if (ownerTeacher.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("The specified teacher account is not active.");
            }

            //if (await _subjectRepository.ExistsByNameForOwnerAsync(trimmedName, finalOwnerId))
            //{
            //    throw new InvalidOperationException($"Subject '{trimmedName}' already exists for this teacher.");
            //}

            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = trimmedName,
                Description = dto.Description?.Trim(),
                OwnerUserId = finalOwnerId,        
                CreatedByUserId = currentUserId,   
                CreatedAt = DateTime.UtcNow
            };

            var created = await subjectRepository.CreateAsync(subject);

            return new SubjectResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                OwnerUserId = created.OwnerUserId,
                CreatedByUserId = created.CreatedByUserId,
                CreatedAt = created.CreatedAt
            };
        }
    }
}
