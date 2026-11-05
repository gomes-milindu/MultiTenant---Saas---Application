using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // 🔒 SuperAdmin පමණක් restrict කරන නිවැරදි ක්‍රමය
    public class SuperAdminController : ControllerBase
    {
        private readonly IBatchService batchService;
        private readonly IPackageService packageService;

        // Constructor Injection
        public SuperAdminController(IBatchService batchService, IPackageService packageService)
        {
            this.batchService = batchService;
            this.packageService = packageService;
        }

        [HttpGet("GetAllBatches")]
        public async Task<IActionResult> GetAll()
        {
            var batches = await batchService.GetAllAsync();
            return Ok(batches);
        }

        [HttpGet("GetPackages,{id:guid}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetPackagesById(Guid id)
        {

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            try
            {
                var result = await packageService.GetPackagesByIdAsync(id);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Retrieve package successfully",
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
