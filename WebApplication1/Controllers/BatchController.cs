using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService batchService;

        public BatchController(IBatchService batchService)
        {
            this.batchService = batchService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBatchDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            var isAdmin = User.IsInRole("SuperAdmin");

            try
            {
                var result = await batchService.CreateAsync(dto, currentUserId, isAdmin);
                return Ok(new
                {
                    message = "Batch created successfully",
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


        //[HttpGet]
        //[Authorize(Roles = "SuperAdmin")] // 🔒 Super Admin ට පමණක් සීමා කර ඇත
        
        //public async Task<IActionResult> GetAll()
        //{
        //    var batches = await batchService.GetAllAsync();
        //    return Ok(batches);
        //}

    }
}
