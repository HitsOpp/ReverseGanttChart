using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;
using ReverseGanttChart.Services.User;

namespace ReverseGanttChart.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        return await _userService.GetAllUsers();
    }
    
    [Authorize]
    [HttpGet("roles")]
    public async Task<IActionResult> GetUserRoles()
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _userService.GetUserRoles(userId);
    }
}