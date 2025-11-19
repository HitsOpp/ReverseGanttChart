using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;
using ReverseGanttChart.Services.Subject;

namespace ReverseGanttChart.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto request)
    {
        var teacherId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _subjectService.CreateSubjectAsync(teacherId, request);
        return Ok(result);
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinSubject([FromBody] JoinSubjectDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var result = await _subjectService.JoinSubjectAsync(userId, request.SubjectId);
        return Ok(result);
    }

    [HttpGet("my-subjects")]
    public async Task<IActionResult> GetMySubjects()
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        var subjects = await _subjectService.GetUserSubjectsAsync(userId);
        return Ok(subjects);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjects = await _subjectService.GetAllSubjectsAsync();
        return Ok(subjects);
    }
}