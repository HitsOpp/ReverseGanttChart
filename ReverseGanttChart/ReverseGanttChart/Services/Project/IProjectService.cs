using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models.Project;
using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Services.Project
{
    public interface IProjectService
    {
        Task<IActionResult> CreateProjectAsync(Guid subjectId, CreateProjectDto request, Guid userId);
        Task<IActionResult> GetSubjectProjectsAsync(Guid subjectId);
        Task<IActionResult> GetProjectAsync(Guid projectId);
        Task<IActionResult> DeleteProjectAsync(Guid projectId, Guid userId);
        
        Task<IActionResult> CreateTaskAsync(Guid projectId, CreateTaskDto request, Guid userId);
        Task<IActionResult> GetProjectTasksAsync(Guid projectId);
        Task<IActionResult> GetTaskAsync(Guid taskId);
        Task<IActionResult> DeleteTaskAsync(Guid taskId, Guid userId);
        
        Task<IActionResult> CreateStageAsync(Guid taskId, CreateStageDto request, Guid userId);
        Task<IActionResult> GetTaskStagesAsync(Guid taskId);
        Task<IActionResult> DeleteStageAsync(Guid stageId, Guid userId);
        
        Task<IActionResult> CompleteTasksForTeamsAsync(CompleteTasksForTeamsDto request, Guid userId);
        Task<IActionResult> CompleteStagesForTeamsAsync(CompleteStagesForTeamsDto request, Guid userId);
        Task<IActionResult> UncompleteTasksForTeamsAsync(UncompleteTasksForTeamsDto request, Guid userId);
        Task<IActionResult> UncompleteStagesForTeamsAsync(UncompleteStagesForTeamsDto request, Guid userId);
        
        
        Task<IActionResult> GetTeamProjectProgressAsync(Guid projectId, Guid teamId);
        Task<IActionResult> GetTeamTaskProgressAsync(Guid taskId, Guid teamId);
        Task<IActionResult> EditProjectAsync(Guid projectId, EditProjectDto request, Guid userId);
        Task<IActionResult> EditTaskAsync(Guid taskId, EditTaskDto request, Guid userId);
        Task<IActionResult> EditStageAsync(Guid stageId, EditStageDto request, Guid userId);
    }
}