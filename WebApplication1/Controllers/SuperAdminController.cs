using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] 
    public class SuperAdminController : ControllerBase
    {
        private readonly IBatchService batchService;
        private readonly IPackageService packageService;
        private readonly ITeacherPackageService teacherPackageService;
        private readonly IAdminMessagingService adminMessagingService;
        private readonly HealthCheckService healthCheckService;

        // Constructor Injection
        public SuperAdminController(IBatchService batchService, IPackageService packageService, ITeacherPackageService teacherPackageService, IAdminMessagingService adminMessagingService, HealthCheckService healthCheckService)
        {
            this.batchService = batchService;
            this.packageService = packageService;
            this.teacherPackageService = teacherPackageService;
            this.adminMessagingService = adminMessagingService;
            this.healthCheckService = healthCheckService;
        }

        [HttpGet("GetAllBatches")]
        public async Task<IActionResult> GetAll()
        {
            var batches = await batchService.GetAllAsync();
            return Ok(batches);
        }


        [HttpGet("GetPackages,{id:guid}")]
        public async Task<IActionResult> GetPackagesById(Guid id)
        {

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid token user identity." });
            }

            var result = await packageService.GetPackagesByIdAsync(id);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Retrieve package successfully",
                data = result
            });

        }


        [HttpGet("GetReminderEmailSettings")]
        public async Task<IActionResult> GetSettings()
        {
            var result = await teacherPackageService.GetSettingsAsync();
            return Ok(result);
        }


        [HttpPut("UpdateReminderEmailSettings")]     
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateReminderSettingsDto dto)
        {
            var result = await teacherPackageService.UpdateSettingsAsync(dto);
            return Ok(new
            {
                success = true,
                message = "Package reminder settings updated successfully.",
                data = result
            });
        }


        [HttpPost("SendMsgtoTeachers")]
        public async Task<IActionResult> SendToTeachers([FromBody] SendAdminTeacherMessageDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idClaim, out var superAdminId))
                return Unauthorized(new { message = "Invalid token user identity." });

            var result = await adminMessagingService.BroadcastToTeachersAsync(dto, superAdminId);
            return Ok(result);

        }


        [HttpGet("HealthCheck")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckHealth(CancellationToken cancellationToken)
        {
            var report = await healthCheckService.CheckHealthAsync(cancellationToken);

            var response = new
            {
                overallStatus = report.Status.ToString(),
                totalDurationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
                services = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    latencyMs = Math.Round(e.Value.Duration.TotalMilliseconds, 2),
                    description = e.Value.Description,
                    error = e.Value.Exception?.Message
                })
            };

            var statusCode = report.Status == HealthStatus.Healthy
                ? StatusCodes.Status200OK
                : StatusCodes.Status503ServiceUnavailable;

            return StatusCode(statusCode, response);
        }
    }
}
