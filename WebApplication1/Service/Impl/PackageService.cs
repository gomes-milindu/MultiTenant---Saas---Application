using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.DTO;
using WebApplication1.Models.Subscriptions;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class PackageService : IPackageService
    {
        public readonly IPackageRepository packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            this.packageRepository = packageRepository;
        }


        public async Task<PackageResponseDto> CreateAsync(CreatePackageDto dto, Guid currentUserId)
        {
            var trimmedCode = dto.Code.Trim().ToUpper();

            if (await packageRepository.ExistsByCodeAsync(trimmedCode))
            {
                throw new InvalidOperationException($"A package with code '{trimmedCode}' already exists.");
            }

            var package = new Package
            {
                Id = Guid.NewGuid(),
                Code = trimmedCode,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                Currency = dto.Currency,
                DurationDays = dto.DurationDays,
                IsFree = dto.IsFree,
                MaxStudents = dto.MaxStudents,
                MaxClasses = dto.MaxClasses,
                MaxBatches = dto.MaxBatches,
                MaxRecordedLessons = dto.MaxRecordedLessons,
                IsActive = true,
                SortOrder = dto.SortOrder,
                CreatedAt = DateTime.UtcNow
            };

            var created = await packageRepository.CreateAsync(package);

            return new PackageResponseDto
            {
                Id = created.Id,
                Code = created.Code,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                Currency = created.Currency,
                DurationDays = created.DurationDays,
                IsFree = created.IsFree,
                MaxStudents = created.MaxStudents,
                MaxClasses = created.MaxClasses,
                MaxBatches = created.MaxBatches,
                MaxRecordedLessons = created.MaxRecordedLessons,
                IsActive = created.IsActive,
                SortOrder = created.SortOrder,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<PackageResponseDto> GetPackagesByIdAsync(Guid id)
        {
            var getpackage = await packageRepository.GetPackagesByIdAsync(id);

            if (getpackage == null)
            {
                throw new KeyNotFoundException($"Package with ID '{id}' was not found.");
            }

            return new PackageResponseDto
            {
                Id = getpackage.Id,
                Code = getpackage.Code,
                Name = getpackage.Name,
                Description = getpackage.Description,
                Price = getpackage.Price,
                Currency = getpackage.Currency,
                DurationDays = getpackage.DurationDays,
                IsFree = getpackage.IsFree,
                MaxStudents = getpackage.MaxStudents,
                MaxClasses = getpackage.MaxClasses,
                MaxBatches = getpackage.MaxBatches,
                MaxRecordedLessons = getpackage.MaxRecordedLessons,
                IsActive = getpackage.IsActive,
                SortOrder = getpackage.SortOrder,
                CreatedAt = getpackage.CreatedAt
            };
        }
    }
}
