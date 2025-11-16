using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Services.Role;

namespace ReverseGanttChart.Controllers;

[Authorize(Roles = "Teacher")]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRolesService _rolesService;

    public RolesController(IRolesService rolesService)
    {
        _rolesService = rolesService;
    }
    

    [Authorize(Roles = "Teacher")]
    [HttpPut("grant-assist/{userId}")]
    public async Task<IActionResult> GrantAssistRole(Guid userId)
    {
        try
        {
            var result = await _rolesService.GrantAssistRoleAsync(userId);
            return Ok(new { message = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}