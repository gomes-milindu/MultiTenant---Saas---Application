using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstitutesController : ControllerBase
    {
        private readonly IInstituteService _instituteService;

        public InstitutesController(IInstituteService instituteService)
        {
            _instituteService = instituteService;
        }

        
        [HttpPost("createInstitute")]
        public async Task<IActionResult> Create([FromBody] CreateInstituteDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            ///currentUserId = idClaim;
            try
            {
                var result = await _instituteService.CreateAsync(dto, currentUserId);
                return Ok(new
                {
                    message = "Institute created successfully",
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
