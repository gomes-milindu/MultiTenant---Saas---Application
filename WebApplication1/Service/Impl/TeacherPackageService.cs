using Microsoft.AspNetCore.Http.HttpResults;
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
            string initialStatus = package.IsFree ? "ACTIVE" : "PENDING";
            DateTime expiresAt = package.IsFree ? activatedAt.AddDays(package.DurationDays) : activatedAt;
            decimal finalPricePaid = package.IsFree ? 0 : dto.PricePaid;

            var teacherPackage = new TeacherPackage
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                PackageId = package.Id,
                Status = initialStatus,
                ActivatedAt = activatedAt,
                ExpiresAt = expiresAt, 
                PricePaid = finalPricePaid,
                CreatedAt = activatedAt
            };

            var created = await teacherPackageRepository.CreateAsync(teacherPackage);

            return new TeacherPackageResponseDto
            {
                Id = teacherPackage.Id,
                UserId = teacherPackage.UserId,
                PackageId = teacherPackage.PackageId,
                Status = teacherPackage.Status,
                ActivatedAt = teacherPackage.ActivatedAt,
                ExpiresAt = teacherPackage.ExpiresAt,
                PricePaid = teacherPackage.PricePaid
            };
        }


        public async Task<bool> ActivatePackageAsync(Guid teacherPackageId)
        {
            var teacherPackage = await teacherPackageRepository.GetByIdAsync(teacherPackageId);

            if (teacherPackage == null || teacherPackage.Status == "ACTIVE")
                return false;

            var now = DateTime.UtcNow;

            // after success payment
            teacherPackage.Status = "ACTIVE";
            teacherPackage.ActivatedAt = now;
            teacherPackage.ExpiresAt = now.AddDays(teacherPackage.Package.DurationDays);

            await teacherPackageRepository.UpdateAsync(teacherPackage);
            return true;
        }

        public async Task<TeacherPackageResponseDto> GetTeacherPackagesByIdAsync(Guid id, Guid currentUserId)
        {
            var teacherPackage = await teacherPackageRepository.GetByIdAsync(id);

            if (teacherPackage == null)
            {
                throw new KeyNotFoundException($"Teacher package with ID was not found.");
            }

            if (teacherPackage.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only view your own package.");

            return new TeacherPackageResponseDto
            {
                Id = teacherPackage.Id,
                UserId = teacherPackage.UserId,
                PackageId = teacherPackage.PackageId,
                Status = teacherPackage.Status,
                ActivatedAt = teacherPackage.ActivatedAt,
                ExpiresAt = teacherPackage.ExpiresAt,
                PricePaid = teacherPackage.PricePaid
            };
        }
    }
}
