using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Services.User;

namespace ReverseGanttChart.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();
        return result;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _userService.GetUserProfileAsync(userId);
        return result;
    }
}