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
    public class SubjectController:ControllerBase
    {

        private readonly ISubjectService subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            this.subjectService = subjectService;
        }

        [HttpPost("CreateSubject")]
        public async Task<IActionResult> Create([FromBody] CreateSubjectDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            var isAdmin = User.IsInRole("SuperAdmin");

            try
            {
                var result = await subjectService.CreateAsync(dto, currentUserId, isAdmin);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Subject created successfully",
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
