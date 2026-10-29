using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // 🔒 SuperAdmin පමණක් restrict කරන නිවැරදි ක්‍රමය
    public class SuperAdminController : ControllerBase
    {
        private readonly IBatchService batchService;

        // Constructor Injection
        public SuperAdminController(IBatchService batchService)
        {
            this.batchService = batchService;
        }

        [HttpGet("GetAllBatches")]
        public async Task<IActionResult> GetAll()
        {
            var batches = await batchService.GetAllAsync();
            return Ok(batches);
        }
    }
}
