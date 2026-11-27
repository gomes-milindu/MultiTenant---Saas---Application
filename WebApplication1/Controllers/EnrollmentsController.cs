using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Service.Auth.Impl;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService enrollmentService;
        private readonly ITeacherProfileService teacherProfileService;

        public EnrollmentsController(IEnrollmentService enrollmentService, ITeacherProfileService teacherProfileService)
        {
            this.enrollmentService = enrollmentService;
            this.teacherProfileService = teacherProfileService;
        }

        [HttpPost("RequestEnrollment")]
        [Authorize(Roles = "Student,SuperAdmin")]
        public async Task<IActionResult> RequestEnrollment([FromBody] CreateEnrollmentDto dto)
        {

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(idClaim, out var studentUserId))
            {
                return Unauthorized(new
                {
                    message = "Invalid token user identity."
                });
            }


            var result = await enrollmentService.CreateAsync(dto, studentUserId);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Enrollment request submitted successfully.",
                data = result
            });

        }


        [HttpGet("ActiveStudentCount/(Teacher-Admin)")]
        [Authorize(Roles = "SuperAdmin,Teacher")]
        public async Task<IActionResult> GetActiveStudentsCount(Guid teacherId)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            Guid targetTeacherId = isAdmin ? teacherId : currentUserId;

            var count = await enrollmentService.GetTotalActiveStudentsCountByTeacherAsync(targetTeacherId);
            return Ok(new { TeacherId = targetTeacherId, ActiveStudentsCount = count });

        }



        [HttpGet("CheckEnrollmentStatus/(Student)")]
        [Authorize(Roles = "Student")] 
        public async Task<IActionResult> CheckEnrollmentStatus([FromQuery] Guid classId)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var studentUserId))
                return Unauthorized();

            var exists = await enrollmentService.ExistsByStudentAndClassAsync(studentUserId, classId);

            return Ok(new { ClassId = classId, IsEnrolledOrPending = exists });
        }



        [HttpGet("ScanTeachersQR")]
        [Authorize]
        public async Task<IActionResult> ScanTeacherQr(Guid teacherId)
        {
            var result = await teacherProfileService.ScanTeacherQrAsync(teacherId);
            if (result == null)
            {
                return NotFound(new { message = "Teacher profile not found." });
            }

            return Ok(result);
        }

    }
}
