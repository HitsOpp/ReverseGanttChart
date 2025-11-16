using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;
using ReverseGanttChart.Services;

namespace ReverseGanttChart.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            var token = await _authService.Register(request);
            return Ok(new { token });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during registration." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        try
        {
            var token = await _authService.Login(request);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login." });
        }
    }
    
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Invalid token: User ID not found.");

        var userId = Guid.Parse(userIdClaim);

        try
        {
            var profile = await _authService.GetProfile(userId);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> EditProfile(EditProfileDto request)
    {
        var userIdClaim = User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("Invalid token: User ID not found.");

        var userId = Guid.Parse(userIdClaim);

        try
        {
            var profile = await _authService.EditProfile(userId, request);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}