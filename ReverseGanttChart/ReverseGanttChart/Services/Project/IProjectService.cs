using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models.Project;

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
        
        Task<IActionResult> CompleteStageAsync(Guid stageId, Guid userId);
        Task<IActionResult> CompleteTaskAsync(Guid taskId, Guid userId);
    }
}