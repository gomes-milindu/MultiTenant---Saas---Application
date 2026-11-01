using WebApplication1.DTO;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Enums;
using WebApplication1.Service.Interface;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Service.Impl
{
    public class EnrollmentService:IEnrollmentService
    {
        private readonly IEnrollmentRepository enrollmentRepository;
        private readonly IClassFeePlanRepository classFeePlanRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IClassFeePlanRepository classFeePlanRepository)
        {
            this.enrollmentRepository = enrollmentRepository;
            this.classFeePlanRepository = classFeePlanRepository;
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

            var currentDate = DateTime.UtcNow;

            var feePlan = await classFeePlanRepository.GetCurrentFeePlanAsync( classDetails.Id, currentDate);

            if (feePlan == null)
            {
                throw new InvalidOperationException("No active fee plan is available for this class.");
            }

            var alreadyEnrolled = await enrollmentRepository.HasActiveEnrollmentAsync( studentUserId,classDetails.Id);

            if (alreadyEnrolled)
            {
                throw new InvalidOperationException("You already have a pending or approved enrollment for this class.");
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
    }
}
