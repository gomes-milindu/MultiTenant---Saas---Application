using WebApplication1.DTO;
using WebApplication1.DTOs.Academic.ClassFeePlan;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;
using WebApplication1.Service.Impl;

namespace WebApplication1.Service.Academic.Impl;

public class ClassFeePlanService : IClassFeePlanService
{
    private readonly IClassFeePlanRepository feePlanRepository;
    private readonly IClassRepository classRepository;

    public ClassFeePlanService(IClassFeePlanRepository feePlanRepository,IClassRepository classRepository)
    {
        this.feePlanRepository = feePlanRepository;
        this.classRepository = classRepository;
    }

    public async Task<ClassFeePlanResponseDto> CreateAsync(CreateClassFeePlanDto dto, Guid currentUserId, bool isAdmin)
    {
        
        var classEntity = await classRepository.GetByIdAsync(dto.ClassId);
        if (classEntity == null)
        {
            throw new KeyNotFoundException("Specified class not found.");
        }

        
        if (classEntity.Status != ClassStatus.Active)
        {
            throw new InvalidOperationException("Cannot add a fee plan to an inactive class.");
        }

        
        if (!isAdmin && classEntity.TeacherUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to set fee plans for this class.");
        }

        if (classEntity.TeacherUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only create fee plans for your own classes.");
        }

        var effectiveFrom = dto.EffectiveFrom ?? DateTime.UtcNow;

        
        var existingActivePlan = await feePlanRepository.GetActivePlanByClassIdAsync(dto.ClassId);
        if (existingActivePlan != null)
        {
            
            if (existingActivePlan.MonthlyFee == dto.MonthlyFee)
            {
                throw new InvalidOperationException($"The active monthly fee for this class is already {dto.MonthlyFee}.");
            }

            
            existingActivePlan.IsActive = false;
            existingActivePlan.EffectiveTo = effectiveFrom;
            existingActivePlan.UpdatedAt = DateTime.UtcNow;

            await feePlanRepository.UpdateAsync(existingActivePlan);
        }

        
        var newFeePlan = new ClassFeePlan
        {
            Id = Guid.NewGuid(),
            ClassId = dto.ClassId,
            MonthlyFee = dto.MonthlyFee,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = null,
            IsActive = true,
            CreatedByUserId = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await feePlanRepository.CreateAsync(newFeePlan);

        var populated = await feePlanRepository.GetByIdAsync(created.Id);

        return new ClassFeePlanResponseDto
        {
            Id = populated!.Id,
            ClassId = populated.ClassId,
            ClassTitle = populated.Class?.Title ?? classEntity.Title,
            MonthlyFee = populated.MonthlyFee,
            EffectiveFrom = populated.EffectiveFrom,
            EffectiveTo = populated.EffectiveTo,
            IsActive = populated.IsActive,
            CreatedByUserId = populated.CreatedByUserId,
            CreatedByUserName = populated.CreatedByUser?.Username ?? string.Empty,
            CreatedAt = populated.CreatedAt
        };
    }

    
}