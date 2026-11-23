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
    public class TeacherProfilesController:ControllerBase
    {
        private readonly ITeacherProfileService teacherProfileService;

        public TeacherProfilesController(ITeacherProfileService teacherProfileService)
        {
            this.teacherProfileService = teacherProfileService;
        }

        [HttpPost("TeacherProfileCreate")]
        [Authorize(Roles = "Teacher,SuperAdmin")]
        public async Task<IActionResult> CreateOrUpdateMyProfile([FromForm] TeacherProfileDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            var isAdmin = User.IsInRole("SuperAdmin");

            if (isAdmin && (dto.TeacherId is null))
            {
                return BadRequest(new { message = "teacherId is required for super admin." });
            }

            try
            {
                var result = await teacherProfileService.CreateOrUpdateMyProfileAsync(dto, currentUserId, isAdmin);
                return Ok(new
                {
                    message = "Teacher profile saved successfully.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
