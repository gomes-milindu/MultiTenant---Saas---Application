using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class EnrollmentService:IEnrollmentService
    {
        private readonly IEnrollmentRepository enrollmentRepository;
        private readonly IClassFeePlanRepository classFeePlanRepository;
        private readonly IUserRepository userRepository;
        private readonly ITeacherPackageRepository teacherPackageRepository;
        private readonly IClassRepository classRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IClassFeePlanRepository classFeePlanRepository, ITeacherPackageRepository teacherPackageRepository, IUserRepository userRepository, IClassRepository classRepository)
        {
            this.enrollmentRepository = enrollmentRepository;
            this.classFeePlanRepository = classFeePlanRepository;
            this.teacherPackageRepository = teacherPackageRepository;
            this.userRepository = userRepository;
            this.classRepository = classRepository;
        }

        public async Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentDto dto,Guid studentUserId)
        {
            if (studentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Invalid student identity.");
            }

            var classDetails = await enrollmentRepository.GetClassForEnrollmentAsync(dto.ClassId);

            if (classDetails == null)
            {
                throw new KeyNotFoundException("Class not found.");
            }

            if (classDetails.Status != ClassStatus.Active)
            {
                throw new InvalidOperationException(
                    "You cannot enroll in an inactive or unavailable class.");
            }

            if (classDetails.Batch == null || classDetails.Batch.Status != BatchStatus.Active)
            {
                throw new InvalidOperationException("You cannot enroll in an inactive or unavailable batch.");
            }

            var alreadyEnrolled = await enrollmentRepository.HasActiveEnrollmentAsync(studentUserId, classDetails.Id);

            if (alreadyEnrolled)
            {
                throw new InvalidOperationException("You already have a pending or approved enrollment for this class.");
            }


            var activePackage = await teacherPackageRepository.GetActivePackageByUserIdAsync(classDetails.TeacherUserId);
            
            if (activePackage == null)
            {
                throw new InvalidOperationException("The teacher of this class does not have an active subscription package. Enrollment paused.");
            }

            if (activePackage.Package.MaxStudents.HasValue)
            {
                var limit = activePackage.Package.MaxStudents.Value;

                var currentStudentsCount = await enrollmentRepository.GetTotalActiveStudentsCountByTeacherAsync(classDetails.TeacherUserId);

                if (currentStudentsCount >= limit)
                {
                    throw new InvalidOperationException($"The teacher's package limit of {limit} students has been reached. Cannot accept new enrollments.");
                }
            }

            var currentDate = DateTime.UtcNow;

            var feePlan = await classFeePlanRepository.GetCurrentFeePlanAsync( classDetails.Id, currentDate);

            if (feePlan == null)
            {
                throw new InvalidOperationException("No active fee plan is available for this class.");
            }

            if (await enrollmentRepository.ExistsByStudentAndClassAsync(studentUserId, dto.ClassId))
            {
                throw new InvalidOperationException("You have already requested to join or are currently enrolled in this class.");
            }


            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),

                StudentUserId = studentUserId,

                ClassId = classDetails.Id,

                BatchId = classDetails.BatchId,

                Status = EnrollmentStatus.Pending,

                RequestedAt = currentDate,

                ApprovedAt = null,

                ApprovedBy = null,

                AgreedMonthlyFee = feePlan.MonthlyFee,

                Notes = string.IsNullOrWhiteSpace(dto.Notes)? null : dto.Notes.Trim()
            };

            var createdEnrollment = await enrollmentRepository.CreateAsync(enrollment);

            return new EnrollmentResponseDto
            {
                Id = createdEnrollment.Id,
                StudentUserId = createdEnrollment.StudentUserId,
                ClassId = createdEnrollment.ClassId,
                BatchId = createdEnrollment.BatchId,
                Status = createdEnrollment.Status,
                RequestedAt = createdEnrollment.RequestedAt,
                ApprovedAt = createdEnrollment.ApprovedAt,
                ApprovedBy = createdEnrollment.ApprovedBy,
                AgreedMonthlyFee = createdEnrollment.AgreedMonthlyFee,
                Notes = createdEnrollment.Notes
            };
        }


        public async Task<int> GetTotalActiveStudentsCountByTeacherAsync(Guid teacherId)
        {
            
            return await enrollmentRepository.GetTotalActiveStudentsCountByTeacherAsync(teacherId);
        }

        public async Task<bool> ExistsByStudentAndClassAsync(Guid studentUserId, Guid classId)
        {
            var classEntity = await classRepository.GetByIdAsync(classId);
            if (classEntity == null)
            {
                throw new KeyNotFoundException("The specified class does not exist.");
            }

            return await enrollmentRepository.ExistsByStudentAndClassAsync(studentUserId, classId);
        }
    }
}
