using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;
using ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Services.Project
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> CreateProjectAsync(Guid subjectId, CreateProjectDto request, Guid userId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                return new NotFoundObjectResult("Subject not found");

            if (!await CanUserManageProjectsAsync(userId, subjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can create projects");

            var project = new Models.Project.Project
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SubjectId = subjectId,
                CreatedById = userId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var projectDto = await GetProjectDtoAsync(project.Id);
            return new OkObjectResult(projectDto);
        }

        public async Task<IActionResult> GetSubjectProjectsAsync(Guid subjectId)
        {
            var projects = await _context.Projects
                .Where(p => p.SubjectId == subjectId)
                .Include(p => p.CreatedBy)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Status = p.Status,
                    CreatedByName = p.CreatedBy.FullName,
                    TaskCount = _context.ProjectTasks.Count(t => t.ProjectId == p.Id),
                    CompletedTaskCount = _context.ProjectTasks.Count(t => t.ProjectId == p.Id && t.Status == ProjectTaskStatus.Completed),
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new OkObjectResult(projects);
        }

        public async Task<IActionResult> GetProjectAsync(Guid projectId)
        {
            var projectDto = await GetProjectDtoAsync(projectId);
            if (projectDto == null)
                return new NotFoundObjectResult("Project not found");

            return new OkObjectResult(projectDto);
        }

        public async Task<IActionResult> DeleteProjectAsync(Guid projectId, Guid userId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return new NotFoundObjectResult("Project not found");

            if (!await CanUserManageProjectsAsync(userId, project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can delete projects");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Project deleted successfully" });
        }

        public async Task<IActionResult> CreateTaskAsync(Guid projectId, CreateTaskDto request, Guid userId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return new NotFoundObjectResult("Project not found");

            if (!await CanUserManageProjectsAsync(userId, project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can create tasks");

            if (request.ParentTaskId.HasValue)
            {
                var parentTask = await _context.ProjectTasks
                    .FirstOrDefaultAsync(t => t.Id == request.ParentTaskId.Value && t.ProjectId == projectId);
                
                if (parentTask == null)
                    return new BadRequestObjectResult("Parent task not found or does not belong to this project");
            }

            var task = new ProjectTask
            {
                Name = request.Name,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                ProjectId = projectId,
                ParentTaskId = request.ParentTaskId
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            var taskDto = await GetTaskDtoAsync(task.Id);
            return new OkObjectResult(taskDto);
        }

        public async Task<IActionResult> GetProjectTasksAsync(Guid projectId)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.ParentTask)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CompletedDate = t.CompletedDate,
                    Status = t.Status,
                    Priority = t.Priority,
                    ParentTaskId = t.ParentTaskId,
                    ParentTaskName = t.ParentTask != null ? t.ParentTask.Name : null,
                    StageCount = _context.TaskStages.Count(s => s.TaskId == t.Id),
                    CompletedStageCount = _context.TaskStages.Count(s => s.TaskId == t.Id && s.IsCompleted),
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new OkObjectResult(tasks);
        }

        public async Task<IActionResult> GetTaskAsync(Guid taskId)
        {
            var taskDto = await GetTaskDtoAsync(taskId);
            if (taskDto == null)
                return new NotFoundObjectResult("Task not found");

            return new OkObjectResult(taskDto);
        }

        public async Task<IActionResult> DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                return new NotFoundObjectResult("Task not found");

            if (!await CanUserManageProjectsAsync(userId, task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can delete tasks");

            var hasSubtasks = await _context.ProjectTasks.AnyAsync(t => t.ParentTaskId == taskId);
            if (hasSubtasks)
                return new BadRequestObjectResult("Cannot delete task that has subtasks. Delete subtasks first.");

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Task deleted successfully" });
        }

        public async Task<IActionResult> CreateStageAsync(Guid taskId, CreateStageDto request, Guid userId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                return new NotFoundObjectResult("Task not found");

            if (!await CanUserManageProjectsAsync(userId, task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can create stages");

            var stage = new TaskStage
            {
                Name = request.Name,
                Description = request.Description,
                EstimatedEffort = request.EstimatedEffort,
                TaskId = taskId
            };

            _context.TaskStages.Add(stage);
            await _context.SaveChangesAsync();

            var stageDto = await GetStageDtoAsync(stage.Id);
            return new OkObjectResult(stageDto);
        }

        public async Task<IActionResult> GetTaskStagesAsync(Guid taskId)
        {
            var stages = await _context.TaskStages
                .Where(s => s.TaskId == taskId)
                .Include(s => s.CompletedBy)
                .Select(s => new StageDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    EstimatedEffort = s.EstimatedEffort,
                    IsCompleted = s.IsCompleted,
                    CompletedAt = s.CompletedAt,
                    CompletedByName = s.CompletedBy != null ? s.CompletedBy.FullName : null,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return new OkObjectResult(stages);
        }

        public async Task<IActionResult> DeleteStageAsync(Guid stageId, Guid userId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.Task)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(s => s.Id == stageId);
            if (stage == null)
                return new NotFoundObjectResult("Stage not found");

            if (!await CanUserManageProjectsAsync(userId, stage.Task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can delete stages");

            _context.TaskStages.Remove(stage);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Stage deleted successfully" });
        }

        public async Task<IActionResult> CompleteStageAsync(Guid stageId, Guid userId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.Task)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(s => s.Id == stageId);
            if (stage == null)
                return new NotFoundObjectResult("Stage not found");

            if (!await CanUserManageProjectsAsync(userId, stage.Task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can complete stages");

            stage.IsCompleted = true;
            stage.CompletedAt = DateTime.UtcNow;
            stage.CompletedById = userId;

            _context.TaskStages.Update(stage);
            await _context.SaveChangesAsync();

            await CheckAndUpdateTaskCompletionAsync(stage.TaskId);

            return new OkObjectResult(new { message = "Stage marked as completed" });
        }

        public async Task<IActionResult> CompleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                return new NotFoundObjectResult("Task not found");

            if (!await CanUserManageProjectsAsync(userId, task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can complete tasks");

            task.Status = ProjectTaskStatus.Completed;
            task.CompletedDate = DateTime.UtcNow;

            _context.ProjectTasks.Update(task);
            await _context.SaveChangesAsync();

            await CheckAndUpdateProjectCompletionAsync(task.ProjectId);

            return new OkObjectResult(new { message = "Task marked as completed" });
        }

        private async Task<bool> CanUserManageProjectsAsync(Guid userId, Guid subjectId)
        {
            var isCreator = await _context.Subjects
                .AnyAsync(s => s.Id == subjectId && s.CreatedById == userId);

            if (isCreator)
                return true;

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId && 
                    (us.Role == SubjectRole.Teacher || us.Role == SubjectRole.Assist));

            return userSubject != null;
        }

        private async Task CheckAndUpdateTaskCompletionAsync(Guid taskId)
        {
            var task = await _context.ProjectTasks
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task != null)
            {
                var allStagesCompleted = await _context.TaskStages
                    .Where(s => s.TaskId == taskId)
                    .AllAsync(s => s.IsCompleted);

                var allSubtasksCompleted = await _context.ProjectTasks
                    .Where(st => st.ParentTaskId == taskId)
                    .AllAsync(st => st.Status == ProjectTaskStatus.Completed);

                if (allStagesCompleted && allSubtasksCompleted)
                {
                    task.Status = ProjectTaskStatus.Completed;
                    task.CompletedDate = DateTime.UtcNow;
                }
                else
                {
                    task.Status = allStagesCompleted || allSubtasksCompleted ? ProjectTaskStatus.InProgress : ProjectTaskStatus.Pending;
                }

                _context.ProjectTasks.Update(task);
                await _context.SaveChangesAsync();

                await CheckAndUpdateProjectCompletionAsync(task.ProjectId);
            }
        }

        private async Task CheckAndUpdateProjectCompletionAsync(Guid projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                var allTasksCompleted = await _context.ProjectTasks
                    .Where(t => t.ProjectId == projectId && t.ParentTaskId == null) 
                    .AllAsync(t => t.Status == ProjectTaskStatus.Completed);

                project.Status = allTasksCompleted ? ProjectStatus.Completed : ProjectStatus.InProgress;

                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<ProjectDto> GetProjectDtoAsync(Guid projectId)
        {
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                CreatedByName = project.CreatedBy.FullName,
                TaskCount = await _context.ProjectTasks.CountAsync(t => t.ProjectId == projectId && t.ParentTaskId == null), 
                CompletedTaskCount = await _context.ProjectTasks.CountAsync(t => t.ProjectId == projectId && t.ParentTaskId == null && t.Status == ProjectTaskStatus.Completed),
                CreatedAt = project.CreatedAt
            };
        }

        private async Task<ProjectTaskDto> GetTaskDtoAsync(Guid taskId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.ParentTask)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                return null;

            return new ProjectTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                Status = task.Status,
                Priority = task.Priority,
                ParentTaskId = task.ParentTaskId,
                ParentTaskName = task.ParentTask?.Name,
                StageCount = await _context.TaskStages.CountAsync(s => s.TaskId == taskId),
                CompletedStageCount = await _context.TaskStages.CountAsync(s => s.TaskId == taskId && s.IsCompleted),
                CreatedAt = task.CreatedAt
            };
        }

        private async Task<StageDto> GetStageDtoAsync(Guid stageId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.CompletedBy)
                .FirstOrDefaultAsync(s => s.Id == stageId);

            if (stage == null)
                return null;

            return new StageDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Description = stage.Description,
                EstimatedEffort = stage.EstimatedEffort,
                IsCompleted = stage.IsCompleted,
                CompletedAt = stage.CompletedAt,
                CompletedByName = stage.CompletedBy?.FullName,
                CreatedAt = stage.CreatedAt
            };
        }
    }
}