using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;



namespace WebApplication1.Service.Impl;

public class BatchService : IBatchService
{
    private readonly IBatchRepository batchRepository;
    private readonly IInstituteRepository instituteRepository;

    public BatchService(IBatchRepository batchRepository, IInstituteRepository instituteRepository )
    {
        this.batchRepository = batchRepository;
        this.instituteRepository = instituteRepository;
    }

    public async Task<BatchResponseDto> CreateAsync(CreateBatchDto dto, Guid currentUserId, bool isAdmin)
    {
        
        var institute = await instituteRepository.GetByIdAsync(dto.InstituteId);
        if (institute == null)
        {
            throw new KeyNotFoundException("Institute not found.");
        }

        
        if (!isAdmin && institute.OwnerUserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to create batches for this institute.");
        }

        if (institute.Status != InstituteStatus.Active)
        {
            throw new InvalidOperationException("The institute is not active.");
        }
        
        var trimmedName = dto.Name.Trim();

        bool excistInstitute = await batchRepository.batchExcistforInstitute(trimmedName, dto.InstituteId);

        if(excistInstitute)
        {
            throw new InvalidOperationException($"A batch named '{trimmedName}' already exists in this institute.");
        }
        

        var batch = new Batch
        {
            Id = Guid.NewGuid(),
            InstituteId = dto.InstituteId,
            Name = trimmedName,
            AcademicYear = dto.AcademicYear,
            Description = dto.Description?.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = BatchStatus.Active,
            CreatedByUserId = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await batchRepository.CreateAsync(batch);

        return new BatchResponseDto
        {
            Id = created.Id,
            InstituteId = created.InstituteId,
            Name = created.Name,
            AcademicYear = created.AcademicYear,
            Description = created.Description,
            StartDate = created.StartDate,
            EndDate = created.EndDate,
            Status = created.Status,
            CreatedByUserId = created.CreatedByUserId,
            CreatedAt = created.CreatedAt
        };
    }
}
