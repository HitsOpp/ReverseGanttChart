using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Services.Subject;

namespace ReverseGanttChart.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectRolesController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectRolesController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpPut("{subjectId}/grant-assist/{userId}")]
    public async Task<IActionResult> GrantAssistRole(Guid subjectId, Guid userId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _subjectService.GrantAssistRoleAsync(subjectId, userId, currentUserId);
        return Ok(result);
    }

    [HttpPut("{subjectId}/revoke-assist/{userId}")]
    public async Task<IActionResult> RevokeAssistRole(Guid subjectId, Guid userId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _subjectService.RevokeAssistRoleAsync(subjectId, userId, currentUserId);
        return Ok(result);
    }
}