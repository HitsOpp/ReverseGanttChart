using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models.Project;
using ReverseGanttChart.Services.Project;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProject(Guid subjectId, [FromBody] CreateProjectDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CreateProjectAsync(subjectId, request, userId);
    }

    [HttpGet("subject-projects")]
    public async Task<IActionResult> GetSubjectProjects(Guid subjectId)
    {
        return await _projectService.GetSubjectProjectsAsync(subjectId);
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        return await _projectService.GetProjectAsync(projectId);
    }

    [HttpDelete("{projectId}/delete")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.DeleteProjectAsync(projectId, userId);
    }

    [HttpPost("{projectId}/tasks/create")]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CreateTaskAsync(projectId, request, userId);
    }

    [HttpGet("{projectId}/tasks")]
    public async Task<IActionResult> GetProjectTasks(Guid projectId)
    {
        return await _projectService.GetProjectTasksAsync(projectId);
    }

    [HttpGet("tasks/{taskId}")]
    public async Task<IActionResult> GetTask(Guid taskId)
    {
        return await _projectService.GetTaskAsync(taskId);
    }

    [HttpDelete("tasks/{taskId}/delete")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.DeleteTaskAsync(taskId, userId);
    }

    [HttpPost("tasks/{taskId}/stages/create")]
    public async Task<IActionResult> CreateStage(Guid taskId, [FromBody] CreateStageDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CreateStageAsync(taskId, request, userId);
    }

    [HttpGet("tasks/{taskId}/stages")]
    public async Task<IActionResult> GetTaskStages(Guid taskId)
    {
        return await _projectService.GetTaskStagesAsync(taskId);
    }

    [HttpDelete("stages/{stageId}/delete")]
    public async Task<IActionResult> DeleteStage(Guid stageId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.DeleteStageAsync(stageId, userId);
    }

    [HttpPost("stages/{stageId}/complete")]
    public async Task<IActionResult> CompleteStage(Guid stageId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CompleteStageAsync(stageId, userId);
    }

    [HttpPost("tasks/{taskId}/complete")]
    public async Task<IActionResult> CompleteTask(Guid taskId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CompleteTaskAsync(taskId, userId);
    }
}