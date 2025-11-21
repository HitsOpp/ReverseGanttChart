using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;

[ApiController]
[Route("api/subjects/{subjectId}/[controller]")]
[Authorize]
public class SubjectManagementController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectManagementController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditSubject(Guid subjectId, [FromBody] EditSubjectDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _subjectService.EditSubjectAsync(subjectId, request, userId);
        return result;
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteSubject(Guid subjectId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _subjectService.DeleteSubjectAsync(subjectId, userId);
        return result;
    }
}