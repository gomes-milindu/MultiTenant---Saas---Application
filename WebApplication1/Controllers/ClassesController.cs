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

    
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClassResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateClassDto dto)
    {
        
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var currentUserId))
        {
            return Unauthorized(new { message = "Invalid token user identity." });
        }

        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

        try
        {
            var result = await classService.CreateAsync(dto, currentUserId, isAdmin);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Class created successfully",
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