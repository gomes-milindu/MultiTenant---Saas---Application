using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            this.enrollmentService = enrollmentService;
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

            try
            {
                var result = await enrollmentService.CreateAsync(dto,studentUserId);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Enrollment request submitted successfully.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = ex.Message
                });
            }
        }
    }
}
