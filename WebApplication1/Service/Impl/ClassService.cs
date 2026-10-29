using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using WebApplication1.Service.Interface;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Impl;

namespace WebApplication1.Service.Impl
{
    public class ClassService : IClassService
    {
        
        private readonly IClassRepository classRepository;
        private readonly IInstituteRepository instituteRepository;
        private readonly IBatchRepository batchRepository;
        private readonly ISubjectRepository subjectRepository;

        public ClassService(IClassRepository classRepository, IInstituteRepository instituteRepository, IBatchRepository batchRepository, ISubjectRepository subjectRepository)
        {
            this.classRepository = classRepository;
            this.instituteRepository = instituteRepository;
            this.batchRepository = batchRepository;
            this.subjectRepository = subjectRepository;
        }


        public async Task<ClassResponseDto> CreateAsync(CreateClassDto dto, Guid currentUserId, bool isAdmin)
        {
            
            var institute = await instituteRepository.GetByIdAsync(dto.InstituteId);
            if (institute == null)
                throw new KeyNotFoundException("Institute not found.");
            
            if (institute.Status != InstituteStatus.Active) 
            {
                throw new InvalidOperationException("Cannot create a class in an inactive or suspended institutes.");
            }

            
            var batch = await batchRepository.GetByIdAsync(dto.BatchId);
            if (batch == null)
                throw new KeyNotFoundException("Batch not found.");

            if (batch.InstituteId != dto.InstituteId)
                throw new InvalidOperationException("The specified batch does not belong to this institute.");

            if (batch.Status != BatchStatus.Active) 
            {
                throw new InvalidOperationException("Cannot create a class in an inactive, completed, or archived batch.");
            }

            
            var subject = await subjectRepository.GetSubjectByIdAsync(dto.SubjectId);
            if (subject == null)
                throw new KeyNotFoundException("Subject not found.");

            
            if (!isAdmin && subject.OwnerUserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You do not own this subject. You cannot create a class for another teacher's subject.");
            }

            Guid assignedTeacherId;
            if (isAdmin)
            {
                if (!dto.TeacherUserId.HasValue || dto.TeacherUserId.Value == Guid.Empty)
                {
                    throw new InvalidOperationException("Admin must select a teacher for this class.");
                }
                assignedTeacherId = dto.TeacherUserId.Value;
            }
            else
            {
                assignedTeacherId = currentUserId;
            }

            
            var trimmedTitle = dto.Title.Trim();
            if (await classRepository.ExistsByTitleInBatchAsync(dto.BatchId, dto.SubjectId, trimmedTitle))
            {
                throw new InvalidOperationException($"A class with the title '{trimmedTitle}' for this subject already exists in this batch.");
            }

            
            var newClass = new Class
            {
                Id = Guid.NewGuid(),
                InstituteId = dto.InstituteId,
                BatchId = dto.BatchId,
                SubjectId = dto.SubjectId,
                TeacherUserId = assignedTeacherId,
                Title = trimmedTitle,
                Schedule = dto.Schedule?.Trim() ?? string.Empty,
                Status = ClassStatus.Active,
                CreatedByUserId = currentUserId, 
                CreatedAt = DateTime.UtcNow
            };

            var created = await classRepository.CreateAsync(newClass);

            return new ClassResponseDto
            {
                Id = created.Id,
                InstituteId = created.InstituteId,
                BatchId = created.BatchId,
                SubjectId = created.SubjectId,
                TeacherUserId = created.TeacherUserId,
                Title = created.Title,
                Schedule = created.Schedule,
                Status = created.Status,
                CreatedByUserId = created.CreatedByUserId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            };
        }
    }
}
