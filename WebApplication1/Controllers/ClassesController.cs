using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Service.Interface;
using WebApplication1.DTO;



namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class ClassesController : ControllerBase
{
    private readonly IClassService classService;

    public ClassesController(IClassService classService)
    {
        this.classService = classService;
    }

    
    [HttpPost("CreateClasses")]
    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateClassDto dto)
    {
        
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var currentUserId))
        {
            return Unauthorized(new { message = "Invalid token user identity." });
        }

        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");


        var result = await classService.CreateAsync(dto, currentUserId, isAdmin);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Class created successfully",
            data = result
        });

    }
}