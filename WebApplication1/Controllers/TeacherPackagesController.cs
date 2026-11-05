using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTO;

using WebApplication1.Service.Interface;


using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeacherPackagesController:ControllerBase
    {
        private readonly ITeacherPackageService teacherPackageService;

        public TeacherPackagesController(ITeacherPackageService teacherPackageService)
        {
            this.teacherPackageService = teacherPackageService;
        }

        [HttpPost("Subscribe")]
        [Authorize(Roles = "Teacher")]

        public async Task<IActionResult> Subscribe([FromBody] CreateTeacherPackageDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            try
            {
                var result = await teacherPackageService.SubscribeAsync(dto, currentUserId);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Successfully subscribed to the package.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("PaymentDone")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSuccessWebhook([FromBody] PaymentWebhookDto payload)
        {
            // Gateway Signature (PayHere/Stripe) Verify in here

            try
            {
                
                var success = await teacherPackageService.ActivatePackageAsync(payload.TeacherPackageId);

                if (success)
                    return Ok(new { message = "Package activated successfully." });

                return BadRequest(new { message = "Package not found or already active." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal error during activation." });
            }
        }


        [HttpGet("GetTeacherPackageById (Logging as a Teacher),{id:guid}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherPackageById(Guid id)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            try
            {
                var result = await teacherPackageService.GetTeacherPackagesByIdAsync(id, currentUserId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
        }
    }
}
