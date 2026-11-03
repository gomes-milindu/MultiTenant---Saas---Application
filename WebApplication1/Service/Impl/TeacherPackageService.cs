using WebApplication1.DTO;
using WebApplication1.Models.Enums;
using WebApplication1.Models.Subscrition;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class TeacherPackageService :ITeacherPackageService
    {
        private readonly ITeacherPackageRepository teacherPackageRepository;
        private readonly IPackageRepository packageRepository;

        private readonly IUserRepository userRepository;

        public TeacherPackageService(ITeacherPackageRepository teacherPackageRepository, IPackageRepository packageRepository, IUserRepository userRepository)
        {
            this.teacherPackageRepository = teacherPackageRepository;
            this.packageRepository = packageRepository;
            this.userRepository = userRepository;
        }

        public async Task<TeacherPackageResponseDto> SubscribeAsync(CreateTeacherPackageDto dto, Guid currentUserId)
        {
            var confirmTeacher = await userRepository.GetUserByIdAsync(currentUserId);
            if (confirmTeacher == null)
            {
                throw new KeyNotFoundException("The specified user does not exist in the system.");
            }

            if (confirmTeacher.Role != UserRole.Teacher)
            {
                throw new UnauthorizedAccessException("Only teachers are allowed to subscribe to these packages.");
            }

            var package = await packageRepository.GetPackagesByIdAsync(dto.PackageId);
            if (package == null || !package.IsActive)
            {
                throw new InvalidOperationException("Invalid or inactive package selected.");
            }

            var existingActive = await teacherPackageRepository.GetActivePackageByUserIdAsync(currentUserId);
            if (existingActive != null)
            {
                throw new InvalidOperationException("You already have an active package.");
            }

            var activatedAt = DateTime.UtcNow;

           
            var teacherPackage = new TeacherPackage
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                PackageId = package.Id,
                Status = "ACTIVE",
                ActivatedAt = activatedAt,
                ExpiresAt = activatedAt.AddDays(package.DurationDays), 
                PricePaid = dto.PricePaid,
                CreatedAt = activatedAt
            };

            var created = await teacherPackageRepository.CreateAsync(teacherPackage);

            return new TeacherPackageResponseDto
            {
                Id = created.Id,
                UserId = created.UserId,
                PackageId = created.PackageId,
                Status = created.Status,
                ActivatedAt = created.ActivatedAt,
                ExpiresAt = created.ExpiresAt,
                PricePaid = created.PricePaid
            };
        }
    }
}
