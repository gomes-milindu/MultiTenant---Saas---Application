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
    }
}
