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

        

        Guid assignedTeacherId;
        if (isAdmin)
        {
            if (!dto.OwnerTeacherId.HasValue || dto.OwnerTeacherId.Value == Guid.Empty)
                throw new InvalidOperationException("SuperAdmin must select a teacher for this batch.");



            assignedTeacherId = dto.OwnerTeacherId.Value;
        }
        else
        {
            // Teacher කෙනෙක් හදන විට Token ID එක Owner ලෙස පත් කරයි
            assignedTeacherId = currentUserId;
        }

        if (institute.OwnerUserId != assignedTeacherId)
        {
            throw new UnauthorizedAccessException("The owner of the batch must match the owner of the institute.");
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
            OwnerTeacherId = assignedTeacherId,
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
            OwnerTeacherId = created.OwnerTeacherId,
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

    public async Task<IEnumerable<BatchResponseDto>> GetAllAsync()
    {
        var batches = await batchRepository.GetAllAsync();

        return batches.Select(b => new BatchResponseDto
        {
            Id = b.Id,
            InstituteId = b.InstituteId,
            OwnerTeacherId = b.OwnerTeacherId,
            OwnerTeacherUserName = b.OwnerTeacher?.Username ?? string.Empty,
            Name = b.Name,
            AcademicYear = b.AcademicYear,
            Description = b.Description,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.Status,
            CreatedByUserId = b.CreatedByUserId,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        });
    }
}
