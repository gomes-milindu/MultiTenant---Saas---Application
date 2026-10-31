

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Models.Auth;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassFeePlansController:ControllerBase
    {
        private readonly IClassFeePlanService classFeePlanService;

        public ClassFeePlansController(IClassFeePlanService classFeePlanService)
        {
            this.classFeePlanService = classFeePlanService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
        public async Task<IActionResult> Create([FromBody] CreateClassFeePlanDto dto)
        {

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Teacher");

            try
            {
                var result = await classFeePlanService.CreateAsync(dto, currentUserId, isAdmin);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Class fee plan created successfully",
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
