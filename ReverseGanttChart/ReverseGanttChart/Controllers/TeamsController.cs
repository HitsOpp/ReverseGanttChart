using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;
using ReverseGanttChart.Services.Team;

[ApiController]
[Route("api/subjects/{subjectId}/[controller]")]
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

    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeam(Guid teamId)
    {
        var result = await _teamService.GetTeamAsync(teamId);
        return result;
    }

    [HttpPut("{teamId}/edit")]
    public async Task<IActionResult> EditTeam(Guid teamId, [FromBody] EditTeamDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.EditTeamAsync(teamId, request, userId);
        return result;
    }

    [HttpDelete("{teamId}/delete")]
    public async Task<IActionResult> DeleteTeam(Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.DeleteTeamAsync(teamId, userId);
        return result;
    }

    [HttpPost("{teamId}/join")]
    public async Task<IActionResult> JoinTeam(Guid teamId, [FromBody] JoinTeamDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.JoinTeamAsync(teamId, request, userId);
        return result;
    }

    [HttpPost("{teamId}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.LeaveTeamAsync(teamId, userId);
        return result;
    }

    [HttpDelete("{teamId}/members/{memberUserId}")]
    public async Task<IActionResult> RemoveTeamMember(Guid teamId, Guid memberUserId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _teamService.RemoveTeamMemberAsync(teamId, memberUserId, currentUserId);
        return result;
    }
}