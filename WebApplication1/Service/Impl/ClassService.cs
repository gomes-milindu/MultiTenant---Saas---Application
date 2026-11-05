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
        private readonly ITeacherPackageRepository teacherPackageRepository;

        public ClassService(IClassRepository classRepository, IInstituteRepository instituteRepository, IBatchRepository batchRepository, ISubjectRepository subjectRepository, ITeacherPackageRepository teacherPackageRepository)
        {
            this.classRepository = classRepository;
            this.instituteRepository = instituteRepository;
            this.batchRepository = batchRepository;
            this.subjectRepository = subjectRepository;
            this.teacherPackageRepository = teacherPackageRepository;
        }


        public async Task<ClassResponseDto> CreateAsync(CreateClassDto dto, Guid currentUserId, bool isAdmin)
        {

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

            var activePackage = await teacherPackageRepository.GetActivePackageByUserIdAsync(assignedTeacherId);
            if (activePackage == null)
            {
                throw new InvalidOperationException("You do not have an active package. Please subscribe to a package before creating a class.");
            }

            if (activePackage.Package.MaxClasses.HasValue)
            {
                var limit = activePackage.Package.MaxClasses.Value;
                var currentClassesCount = await classRepository.GetTotalClassesCountByTeacherAsync(assignedTeacherId);

                if (currentClassesCount >= limit)
                {
                    throw new InvalidOperationException($"Your package limit of {limit} classes has been reached. Please upgrade your package to create more.");
                }
            }

            var institute = await instituteRepository.GetByIdAsync(dto.InstituteId);
            if (institute == null)
                throw new KeyNotFoundException("Institute not found.");

            if (institute.Status != InstituteStatus.Active)
                throw new InvalidOperationException("Cannot create a class in an inactive or suspended institute.");

            if (!isAdmin && institute.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to create classes in this institute.");

            var batch = await batchRepository.GetByIdAsync(dto.BatchId);
            if (batch == null)
                throw new KeyNotFoundException("Batch not found.");

            if (batch.InstituteId != dto.InstituteId)
                throw new InvalidOperationException("The specified batch does not belong to this institute.");

            if (batch.Status != BatchStatus.Active)
                throw new InvalidOperationException("Cannot create a class in an inactive, completed, or archived batch.");

     
            var subject = await subjectRepository.GetSubjectByIdAsync(dto.SubjectId);
            if (subject == null)
                throw new KeyNotFoundException("Subject not found.");

            
            if (subject.OwnerUserId != assignedTeacherId)
            {
                throw new UnauthorizedAccessException("The assigned teacher does not own this subject. Cannot create class.");
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
