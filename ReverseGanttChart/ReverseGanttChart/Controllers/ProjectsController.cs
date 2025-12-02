using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models.Project;
using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;
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

    [HttpGet]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        return await _projectService.GetProjectAsync(projectId);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.DeleteProjectAsync(projectId, userId);
    }
    
    [HttpPost("/tasks/create")]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CreateTaskAsync(projectId, request, userId);
    }

    [HttpGet("/tasks")]
    public async Task<IActionResult> GetProjectTasks(Guid projectId)
    {
        return await _projectService.GetProjectTasksAsync(projectId);
    }

    [HttpGet("/tasks/info")]
    public async Task<IActionResult> GetTask(Guid taskId)
    {
        return await _projectService.GetTaskAsync(taskId);
    }

    [HttpDelete("tasks/delete")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.DeleteTaskAsync(taskId, userId);
    }

    [HttpPost("tasks/stages/create")]
    public async Task<IActionResult> CreateStage(Guid taskId, [FromBody] CreateStageDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CreateStageAsync(taskId, request, userId);
    }

    [HttpGet("tasks/stages")]
    public async Task<IActionResult> GetTaskStages(Guid taskId)
    {
        return await _projectService.GetTaskStagesAsync(taskId);
    }

    [HttpDelete("stages/delete")]
    public async Task<IActionResult> DeleteStage(Guid stageId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.DeleteStageAsync(stageId, userId);
    }

    [HttpPost("stages/complete-for-team")]
    public async Task<IActionResult> CompleteStageForTeam(Guid stageId, Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CompleteStageForTeamAsync(stageId, teamId, userId);
    }

    [HttpPost("stages/uncomplete-for-team")]
    public async Task<IActionResult> UncompleteStageForTeam(Guid stageId, Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.UncompleteStageForTeamAsync(stageId, teamId, userId);
    }

    [HttpPost("tasks/complete-for-team")]
    public async Task<IActionResult> CompleteTaskForTeam(Guid taskId, Guid teamId)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.CompleteTaskForTeamAsync(taskId, teamId, userId);
    }

    [HttpGet("Projects/team-progress")]
    public async Task<IActionResult> GetTeamProjectProgress(Guid projectId, Guid teamId)
    {
        return await _projectService.GetTeamProjectProgressAsync(projectId, teamId);
    }

    [HttpGet("tasks/team-progress")]
    public async Task<IActionResult> GetTeamTaskProgress(Guid taskId, Guid teamId)
    {
        return await _projectService.GetTeamTaskProgressAsync(taskId, teamId);
    }
    
    [HttpPut("edit")]
    public async Task<IActionResult> EditProject(Guid projectId, [FromBody] EditProjectDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.EditProjectAsync(projectId, request, userId);
    }
    [HttpPut("tasks/edit")]
    public async Task<IActionResult> EditTask(Guid taskId, [FromBody] EditTaskDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.EditTaskAsync(taskId, request, userId);
    }
    
    [HttpPut("stages/edit")]
    public async Task<IActionResult> EditStage(Guid stageId, [FromBody] EditStageDto request)
    {
        var userId = Guid.Parse(User.FindFirst("Id")?.Value);
        return await _projectService.EditStageAsync(stageId, request, userId);
    }
}