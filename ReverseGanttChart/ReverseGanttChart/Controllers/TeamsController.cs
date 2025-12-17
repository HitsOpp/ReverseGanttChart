using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;
using ReverseGanttChart.Services.Team;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTeam(Guid subjectId, [FromBody] CreateTeamDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.CreateTeamAsync(subjectId, request, userId);
        return result;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetSubjectTeams(Guid subjectId)
    {
        var result = await _teamService.GetSubjectTeamsAsync(subjectId);
        return result;
    }

    [HttpGet("information")]
    public async Task<IActionResult> GetTeam(Guid teamId)
    {
        var result = await _teamService.GetTeamAsync(teamId);
        return result;
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditTeam(Guid teamId, [FromBody] EditTeamDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.EditTeamAsync(teamId, request, userId);
        return result;
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteTeam(Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.DeleteTeamAsync(teamId, userId);
        return result;
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinTeam(Guid teamId, [FromBody] JoinTeamDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.JoinTeamAsync(teamId, request, userId);
        return result;
    }

    [HttpPost("leave")]
    public async Task<IActionResult> LeaveTeam(Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.LeaveTeamAsync(teamId, userId);
        return result;
    }

    [HttpDelete("members/remove")]
    public async Task<IActionResult> RemoveTeamMember(Guid teamId, Guid memberUserId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.RemoveTeamMemberAsync(teamId, memberUserId, currentUserId);
        return result;
    }

    [HttpGet("my-team")]
    public async Task<IActionResult> GetMyTeamInSubject(Guid subjectId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.GetUserTeamInSubjectAsync(subjectId, userId);
        return result;
    }

    [HttpGet("user/team")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetUserTeamInSubject(Guid subjectId, Guid userId)
    {
        var result = await _teamService.GetUserTeamInSubjectAsync(subjectId, userId);
        return result;
    }
    
}