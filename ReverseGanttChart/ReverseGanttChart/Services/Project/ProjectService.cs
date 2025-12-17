using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;
using ReverseGanttChart.Models.Project;
using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

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

            await InitializeTeamProgressForProjectAsync(project.Id, subjectId);

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
                    CreatedByName = p.CreatedBy.FullName,
                    TaskCount = _context.ProjectTasks.Count(t => t.ProjectId == p.Id),
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

            var task = new ProjectTask
            {
                Name = request.Name,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                ProjectId = projectId
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            await InitializeTeamProgressForTaskAsync(task.Id, project.SubjectId);

            var taskDto = await GetTaskDtoAsync(task.Id);
            return new OkObjectResult(taskDto);
        }

        public async Task<IActionResult> GetProjectTasksAsync(Guid projectId)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    TeamProgress = _context.Teams
                        .Where(team => team.SubjectId == t.Project.SubjectId)
                        .Select(team => new
                        {
                            TeamId = team.Id,
                            TeamName = team.Name,
                            Status = _context.TeamTaskProgress
                                .Where(ttp => ttp.TaskId == t.Id && ttp.TeamId == team.Id)
                                .Select(ttp => ttp.Status)
                                .FirstOrDefault(),
                            CompletedDate = _context.TeamTaskProgress
                                .Where(ttp => ttp.TaskId == t.Id && ttp.TeamId == team.Id)
                                .Select(ttp => ttp.CompletedDate)
                                .FirstOrDefault(),
                            CompletedByName = _context.TeamTaskProgress
                                .Where(ttp => ttp.TaskId == t.Id && ttp.TeamId == team.Id)
                                .Select(ttp => ttp.CompletedBy.FullName)
                                .FirstOrDefault(),
                            CompletedStageCount = _context.TeamStageProgress
                                .Count(tsp => tsp.Stage.TaskId == t.Id && tsp.TeamId == team.Id && tsp.IsCompleted),
                            TotalStageCount = _context.TaskStages.Count(s => s.TaskId == t.Id)
                        }).ToList()
                })
                .ToListAsync();

            var result = tasks.Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.DueDate,
                t.Priority,
                t.CreatedAt,
                TeamProgress = t.TeamProgress.Select(tp => new
                {
                    tp.TeamId,
                    tp.TeamName,
                    tp.Status,
                    tp.CompletedDate,
                    tp.CompletedByName,
                    tp.CompletedStageCount,
                    tp.TotalStageCount,
                    Progress = tp.TotalStageCount > 0 ? (double)tp.CompletedStageCount / tp.TotalStageCount * 100 : 0
                }).ToList()
            }).ToList();

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> GetTaskAsync(Guid taskId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                return new NotFoundObjectResult("Task not found");

            var teamProgress = await _context.Teams
                .Where(team => team.SubjectId == task.Project.SubjectId)
                .Select(team => new
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    Status = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id)
                        .Select(ttp => ttp.Status)
                        .FirstOrDefault(),
                    CompletedDate = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id)
                        .Select(ttp => ttp.CompletedDate)
                        .FirstOrDefault(),
                    CompletedByName = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id)
                        .Select(ttp => ttp.CompletedBy.FullName)
                        .FirstOrDefault(),
                    CompletedStageCount = _context.TeamStageProgress
                        .Count(tsp => tsp.Stage.TaskId == taskId && tsp.TeamId == team.Id && tsp.IsCompleted),
                    TotalStageCount = _context.TaskStages.Count(s => s.TaskId == taskId)
                }).ToListAsync();

            var result = new
            {
                task.Id,
                task.Name,
                task.Description,
                task.DueDate,
                task.Priority,
                task.CreatedAt,
                TeamProgress = teamProgress.Select(tp => new
                {
                    tp.TeamId,
                    tp.TeamName,
                    tp.Status,
                    tp.CompletedDate,
                    tp.CompletedByName,
                    tp.CompletedStageCount,
                    tp.TotalStageCount,
                    Progress = tp.TotalStageCount > 0 ? (double)tp.CompletedStageCount / tp.TotalStageCount * 100 : 0
                }).ToList()
            };

            return new OkObjectResult(result);
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

            await InitializeTeamProgressForStageAsync(stage.Id, task.Project.SubjectId);

            var stageDto = await GetStageDtoAsync(stage.Id);
            return new OkObjectResult(stageDto);
        }

        public async Task<IActionResult> GetTaskStagesAsync(Guid taskId)
        {
            var stages = await _context.TaskStages
                .Where(s => s.TaskId == taskId)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    EstimatedEffort = s.EstimatedEffort,
                    CreatedAt = s.CreatedAt,
                    TeamProgress = _context.Teams
                        .Where(team => team.SubjectId == s.Task.Project.SubjectId)
                        .Select(team => new
                        {
                            TeamId = team.Id,
                            TeamName = team.Name,
                            IsCompleted = _context.TeamStageProgress
                                .Any(tsp => tsp.StageId == s.Id && tsp.TeamId == team.Id && tsp.IsCompleted),
                            CompletedAt = _context.TeamStageProgress
                                .Where(tsp => tsp.StageId == s.Id && tsp.TeamId == team.Id)
                                .Select(tsp => tsp.CompletedAt)
                                .FirstOrDefault(),
                            CompletedByName = _context.TeamStageProgress
                                .Where(tsp => tsp.StageId == s.Id && tsp.TeamId == team.Id)
                                .Select(tsp => tsp.CompletedBy.FullName)
                                .FirstOrDefault()
                        }).ToList()
                })
                .ToListAsync();

            var result = stages.Select(s => new
            {
                s.Id,
                s.Name,
                s.Description,
                s.EstimatedEffort,
                s.CreatedAt,
                TeamProgress = s.TeamProgress.Select(tp => new
                {
                    tp.TeamId,
                    tp.TeamName,
                    tp.IsCompleted,
                    tp.CompletedAt,
                    tp.CompletedByName
                }).ToList()
            }).ToList();

            return new OkObjectResult(result);
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

        public async Task<IActionResult> CompleteStagesForTeamsAsync(CompleteStagesForTeamsDto request, Guid userId)
        {
            if (!request.TeamIds.Any() || !request.StageIds.Any())
                return new BadRequestObjectResult("TeamIds and StageIds cannot be empty");

            var results = new List<object>();
            var errors = new List<string>();

            foreach (var teamId in request.TeamIds)
            {
                foreach (var stageId in request.StageIds)
                {
                    try
                    {
                        var result = await CompleteSingleStageForTeamAsync(stageId, teamId, userId);
                        results.Add(new
                        {
                            TeamId = teamId,
                            StageId = stageId,
                            Success = true,
                            Message = $"Stage completed for team"
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Stage {stageId} for team {teamId}: {ex.Message}");
                        results.Add(new
                        {
                            TeamId = teamId,
                            StageId = stageId,
                            Success = false,
                            Error = ex.Message
                        });
                    }
                }
            }

            return new OkObjectResult(new
            {
                Results = results
            });
        }

        public async Task<IActionResult> UncompleteStagesForTeamsAsync(UncompleteStagesForTeamsDto request, Guid userId)
        {
            if (!request.TeamIds.Any() || !request.StageIds.Any())
                return new BadRequestObjectResult("TeamIds and StageIds cannot be empty");

            var results = new List<object>();
            var errors = new List<string>();

            foreach (var teamId in request.TeamIds)
            {
                foreach (var stageId in request.StageIds)
                {
                    try
                    {
                        var result = await UncompleteSingleStageForTeamAsync(stageId, teamId, userId);
                        results.Add(new
                        {
                            TeamId = teamId,
                            StageId = stageId,
                            Success = true,
                            Message = $"Stage uncompleted for team"
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Stage {stageId} for team {teamId}: {ex.Message}");
                        results.Add(new
                        {
                            TeamId = teamId,
                            StageId = stageId,
                            Success = false,
                            Error = ex.Message
                        });
                    }
                }
            }

            return new OkObjectResult(new
            {
                Results = results
            });
        }

        public async Task<IActionResult> CompleteTasksForTeamsAsync(CompleteTasksForTeamsDto request, Guid userId)
        {
            if (!request.TeamIds.Any() || !request.TaskIds.Any())
                return new BadRequestObjectResult("TeamIds and TaskIds cannot be empty");

            var results = new List<object>();
            var errors = new List<string>();

            foreach (var teamId in request.TeamIds)
            {
                foreach (var taskId in request.TaskIds)
                {
                    try
                    {
                        var result = await CompleteSingleTaskForTeamAsync(taskId, teamId, userId);
                        results.Add(new
                        {
                            TeamId = teamId,
                            TaskId = taskId,
                            Success = true,
                            Message = $"Task completed for team"
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Task {taskId} for team {teamId}: {ex.Message}");
                        results.Add(new
                        {
                            TeamId = teamId,
                            TaskId = taskId,
                            Success = false,
                            Error = ex.Message
                        });
                    }
                }
            }

            return new OkObjectResult(new
            {
                Results = results
            });
        }

        public async Task<IActionResult> GetTeamProjectProgressAsync(Guid projectId, Guid teamId)
        {
            var project = await _context.Projects
                .Where(p => p.Id == projectId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.SubjectId
                })
                .FirstOrDefaultAsync();
            
            if (project == null)
                return new NotFoundObjectResult("Project not found");

            var team = await _context.Teams
                .Where(t => t.Id == teamId)
                .Select(t => new { t.Id, t.Name, t.SubjectId })
                .FirstOrDefaultAsync();
            
            if (team == null || team.SubjectId != project.SubjectId)
                return new NotFoundObjectResult("Team not found or not in the same subject");

            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new
                {
                    Task = new
                    {
                        t.Id,
                        t.Name,
                        t.Description,
                        t.DueDate,
                        t.Priority
                    },
                    TaskProgress = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == t.Id && ttp.TeamId == teamId)
                        .Select(ttp => new
                        {
                            ttp.Status,
                            ttp.CompletedDate,
                            CompletedByName = ttp.CompletedBy != null ? ttp.CompletedBy.FullName : null
                        })
                        .FirstOrDefault(),
                    CompletedStages = _context.TeamStageProgress
                        .Count(tsp => tsp.Stage.TaskId == t.Id && tsp.TeamId == teamId && tsp.IsCompleted),
                    TotalStages = _context.TaskStages.Count(s => s.TaskId == t.Id)
                })
                .ToListAsync();

            var tasksDto = tasks.Select(t => new
            {
                t.Task,
                TaskProgress = t.TaskProgress,
                t.CompletedStages,
                t.TotalStages,
                Progress = t.TotalStages > 0 ? (double)t.CompletedStages / t.TotalStages * 100 : 0,
            }).ToList();

            var overallProgress = tasksDto.Any() ? tasksDto.Average(t => t.Progress) : 0;

            var result = new
            {
                Project = new
                {
                    project.Id,
                    project.Name,
                    project.Description
                },
                Team = new
                {
                    team.Id,
                    team.Name
                },
                Tasks = tasksDto,
                OverallProgress = overallProgress
            };

            return new OkObjectResult(result);
        }

        public async Task<IActionResult> GetTeamTaskProgressAsync(Guid taskId, Guid teamId)
        {
            var task = await _context.ProjectTasks
                .Where(t => t.Id == taskId)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                    t.DueDate,
                    t.Priority,
                    ProjectSubjectId = t.Project.SubjectId
                })
                .FirstOrDefaultAsync();
            
            if (task == null)
                return new NotFoundObjectResult("Task not found");

            var team = await _context.Teams
                .Where(t => t.Id == teamId)
                .Select(t => new { t.Id, t.Name })
                .FirstOrDefaultAsync();
            
            if (team == null || !await _context.Teams.AnyAsync(t => t.Id == teamId && t.SubjectId == task.ProjectSubjectId))
                return new NotFoundObjectResult("Team not found or not in the same subject");

            var taskProgress = await _context.TeamTaskProgress
                .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == teamId)
                .Select(ttp => new
                {
                    ttp.Status,
                    ttp.CompletedDate,
                    CompletedByName = ttp.CompletedBy != null ? ttp.CompletedBy.FullName : null
                })
                .FirstOrDefaultAsync();

            var stages = await _context.TaskStages
                .Where(s => s.TaskId == taskId)
                .Select(s => new
                {
                    Stage = new
                    {
                        s.Id,
                        s.Name,
                        s.Description,
                        s.EstimatedEffort
                    },
                    Progress = _context.TeamStageProgress
                        .Where(tsp => tsp.StageId == s.Id && tsp.TeamId == teamId)
                        .Select(tsp => new
                        {
                            tsp.IsCompleted,
                            tsp.CompletedAt,
                            CompletedByName = tsp.CompletedBy != null ? tsp.CompletedBy.FullName : null
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            var completedStages = stages.Count(s => s.Progress != null && s.Progress.IsCompleted);
            var totalStages = stages.Count;
            var progress = totalStages > 0 ? (double)completedStages / totalStages * 100 : 0;
            
            var result = new
            {
                Task = new
                {
                    task.Id,
                    task.Name,
                    task.Description,
                    task.DueDate,
                    task.Priority
                },
                Team = new
                {
                    team.Id,
                    team.Name
                },
                TaskProgress = taskProgress != null ? new
                {
                    Status = taskProgress.Status,
                    CompletedDate = taskProgress.CompletedDate,
                    CompletedByName = taskProgress.CompletedByName
                } : null,
                Stages = stages.Select(s => new
                {
                    Stage = s.Stage,
                    Progress = s.Progress != null ? new
                    {
                        s.Progress.IsCompleted,
                        s.Progress.CompletedAt,
                        s.Progress.CompletedByName
                    } : new
                    {
                        IsCompleted = false,
                        CompletedAt = (DateTime?)null,
                        CompletedByName = (string)null
                    }
                }),
                Progress = progress
            };

            return new OkObjectResult(result);
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

        private async Task InitializeTeamProgressForProjectAsync(Guid projectId, Guid subjectId)
        {
            var teams = await _context.Teams
                .Where(t => t.SubjectId == subjectId)
                .ToListAsync();

            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            foreach (var team in teams)
            {
                foreach (var task in tasks)
                {
                    var existingProgress = await _context.TeamTaskProgress
                        .AnyAsync(ttp => ttp.TaskId == task.Id && ttp.TeamId == team.Id);

                    if (!existingProgress)
                    {
                        var teamTaskProgress = new TeamTaskProgress
                        {
                            TaskId = task.Id,
                            TeamId = team.Id,
                            Status = ProjectTaskStatus.Pending
                        };
                        _context.TeamTaskProgress.Add(teamTaskProgress);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task InitializeTeamProgressForTaskAsync(Guid taskId, Guid subjectId)
        {
            var teams = await _context.Teams
                .Where(t => t.SubjectId == subjectId)
                .ToListAsync();

            foreach (var team in teams)
            {
                var existingProgress = await _context.TeamTaskProgress
                    .AnyAsync(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id);

                if (!existingProgress)
                {
                    var teamTaskProgress = new TeamTaskProgress
                    {
                        TaskId = taskId,
                        TeamId = team.Id,
                        Status = ProjectTaskStatus.Pending
                    };
                    _context.TeamTaskProgress.Add(teamTaskProgress);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task InitializeTeamProgressForStageAsync(Guid stageId, Guid subjectId)
        {
            var teams = await _context.Teams
                .Where(t => t.SubjectId == subjectId)
                .ToListAsync();

            foreach (var team in teams)
            {
                var existingProgress = await _context.TeamStageProgress
                    .AnyAsync(tsp => tsp.StageId == stageId && tsp.TeamId == team.Id);

                if (!existingProgress)
                {
                    var teamStageProgress = new TeamStageProgress
                    {
                        StageId = stageId,
                        TeamId = team.Id,
                        IsCompleted = false
                    };
                    _context.TeamStageProgress.Add(teamStageProgress);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CheckAndUpdateTaskCompletionForTeamAsync(Guid taskId, Guid teamId)
        {
            var teamTaskProgress = await _context.TeamTaskProgress
                .FirstOrDefaultAsync(ttp => ttp.TaskId == taskId && ttp.TeamId == teamId);

            if (teamTaskProgress != null)
            {
                var allStagesCompleted = await _context.TeamStageProgress
                    .Where(tsp => tsp.Stage.TaskId == taskId && tsp.TeamId == teamId)
                    .AllAsync(tsp => tsp.IsCompleted);

                if (teamTaskProgress.Status == ProjectTaskStatus.Completed && !allStagesCompleted)
                {
                    teamTaskProgress.Status = ProjectTaskStatus.InProgress;
                    teamTaskProgress.CompletedDate = null;
                    teamTaskProgress.CompletedById = null;
                }
                else if (teamTaskProgress.Status != ProjectTaskStatus.Completed)
                {
                    if (allStagesCompleted)
                    {
                        teamTaskProgress.Status = ProjectTaskStatus.InProgress;
                    }
                    else
                    {
                        teamTaskProgress.Status = allStagesCompleted ? 
                            ProjectTaskStatus.InProgress : ProjectTaskStatus.Pending;
                    }
                }

                _context.TeamTaskProgress.Update(teamTaskProgress);
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
                CreatedByName = project.CreatedBy.FullName,
                TaskCount = await _context.ProjectTasks.CountAsync(t => t.ProjectId == projectId),
                CreatedAt = project.CreatedAt
            };
        }

        private async Task<ProjectTaskDto> GetTaskDtoAsync(Guid taskId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                return null;

            var teamProgress = await _context.Teams
                .Where(team => team.SubjectId == task.Project.SubjectId)
                .Select(team => new TeamProgressDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    Status = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id)
                        .Select(ttp => ttp.Status)
                        .FirstOrDefault(),
                    CompletedDate = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id)
                        .Select(ttp => ttp.CompletedDate)
                        .FirstOrDefault(),
                    CompletedByName = _context.TeamTaskProgress
                        .Where(ttp => ttp.TaskId == taskId && ttp.TeamId == team.Id)
                        .Select(ttp => ttp.CompletedBy.FullName)
                        .FirstOrDefault(),
                    CompletedStageCount = _context.TeamStageProgress
                        .Count(tsp => tsp.Stage.TaskId == taskId && tsp.TeamId == team.Id && tsp.IsCompleted),
                    TotalStageCount = _context.TaskStages.Count(s => s.TaskId == taskId)
                }).ToListAsync();

            var teamProgressWithProgress = teamProgress.Select(tp => new
            {
                tp.TeamId,
                tp.TeamName,
                tp.Status,
                tp.CompletedDate,
                tp.CompletedByName,
                tp.CompletedStageCount,
                tp.TotalStageCount,
                Progress = tp.TotalStageCount > 0 ? (double)tp.CompletedStageCount / tp.TotalStageCount * 100 : 0
            }).ToList();

            return new ProjectTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                TeamProgress = teamProgress.Select(tp => new TeamProgressDto
                {
                    TeamId = tp.TeamId,
                    TeamName = tp.TeamName,
                    Status = tp.Status,
                    CompletedDate = tp.CompletedDate,
                    CompletedByName = tp.CompletedByName,
                    CompletedStageCount = tp.CompletedStageCount,
                    TotalStageCount = tp.TotalStageCount
                }).ToList(),
                CreatedAt = task.CreatedAt
            };
        }

        private async Task<StageDto> GetStageDtoAsync(Guid stageId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.Task)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(s => s.Id == stageId);

            if (stage == null)
                return null;

            var teamProgress = await _context.Teams
                .Where(team => team.SubjectId == stage.Task.Project.SubjectId)
                .Select(team => new TeamStageProgressDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    IsCompleted = _context.TeamStageProgress
                        .Any(tsp => tsp.StageId == stageId && tsp.TeamId == team.Id && tsp.IsCompleted),
                    CompletedAt = _context.TeamStageProgress
                        .Where(tsp => tsp.StageId == stageId && tsp.TeamId == team.Id)
                        .Select(tsp => tsp.CompletedAt)
                        .FirstOrDefault(),
                    CompletedByName = _context.TeamStageProgress
                        .Where(tsp => tsp.StageId == stageId && tsp.TeamId == team.Id)
                        .Select(tsp => tsp.CompletedBy.FullName)
                        .FirstOrDefault()
                }).ToListAsync();

            return new StageDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Description = stage.Description,
                EstimatedEffort = stage.EstimatedEffort,
                TeamProgress = teamProgress,
                CreatedAt = stage.CreatedAt
            };
        }

        public async Task<IActionResult> EditProjectAsync(Guid projectId, EditProjectDto request, Guid userId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return new NotFoundObjectResult("Project not found");

            if (!await CanUserManageProjectsAsync(userId, project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can edit projects");

            if (request.EndDate < request.StartDate)
                return new BadRequestObjectResult("End date cannot be earlier than start date");

            project.Name = request.Name;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            var projectDto = await GetProjectDtoAsync(project.Id);
            return new OkObjectResult(projectDto);
        }

        public async Task<IActionResult> EditTaskAsync(Guid taskId, EditTaskDto request, Guid userId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);
            
            if (task == null)
                return new NotFoundObjectResult("Task not found");

            if (!await CanUserManageProjectsAsync(userId, task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can edit tasks");

            task.Name = request.Name;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.Priority = request.Priority;

            _context.ProjectTasks.Update(task);
            await _context.SaveChangesAsync();

            var taskDto = await GetTaskDtoAsync(task.Id);
            return new OkObjectResult(taskDto);
        }

        public async Task<IActionResult> EditStageAsync(Guid stageId, EditStageDto request, Guid userId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.Task)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(s => s.Id == stageId);
            
            if (stage == null)
                return new NotFoundObjectResult("Stage not found");

            if (!await CanUserManageProjectsAsync(userId, stage.Task.Project.SubjectId))
                return new UnauthorizedObjectResult("Only teachers and assists can edit stages");

            if (request.EstimatedEffort < 0)
                return new BadRequestObjectResult("Estimated effort cannot be negative");

            stage.Name = request.Name;
            stage.Description = request.Description;
            stage.EstimatedEffort = request.EstimatedEffort;

            _context.TaskStages.Update(stage);
            await _context.SaveChangesAsync();

            var stageDto = await GetStageDtoAsync(stage.Id);
            return new OkObjectResult(stageDto);
        }
        public async Task<IActionResult> UncompleteTasksForTeamsAsync(UncompleteTasksForTeamsDto request, Guid userId)
        {
            if (!request.TeamIds.Any() || !request.TaskIds.Any())
                return new BadRequestObjectResult("TeamIds and TaskIds cannot be empty");

            var results = new List<object>();
            var errors = new List<string>();

            foreach (var teamId in request.TeamIds)
            {
                foreach (var taskId in request.TaskIds)
                {
                    try
                    {
                        var result = await UncompleteSingleTaskForTeamAsync(taskId, teamId, userId);
                        results.Add(new
                        {
                            TeamId = teamId,
                            TaskId = taskId,
                            Success = true,
                            Message = $"Task uncompleted for team"
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Task {taskId} for team {teamId}: {ex.Message}");
                        results.Add(new
                        {
                            TeamId = teamId,
                            TaskId = taskId,
                            Success = false,
                            Error = ex.Message
                        });
                    }
                }
            }

            return new OkObjectResult(new
            {
                Results = results
            });
        }
        private async Task<object> CompleteSingleTaskForTeamAsync(Guid taskId, Guid teamId, Guid userId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new Exception("Task not found");

            if (!await CanUserManageProjectsAsync(userId, task.Project.SubjectId))
                throw new Exception("Only teachers and assists can complete tasks");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null || team.SubjectId != task.Project.SubjectId)
                throw new Exception("Team not found or not in the same subject");

            var stages = await _context.TaskStages
                .Where(s => s.TaskId == taskId)
                .ToListAsync();

            foreach (var stage in stages)
            {
                var teamStageProgress = await _context.TeamStageProgress
                    .FirstOrDefaultAsync(tsp => tsp.StageId == stage.Id && tsp.TeamId == teamId);

                if (teamStageProgress == null)
                {
                    teamStageProgress = new TeamStageProgress
                    {
                        StageId = stage.Id,
                        TeamId = teamId,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow,
                        CompletedById = userId
                    };
                    _context.TeamStageProgress.Add(teamStageProgress);
                }
                else if (!teamStageProgress.IsCompleted)
                {
                    teamStageProgress.IsCompleted = true;
                    teamStageProgress.CompletedAt = DateTime.UtcNow;
                    teamStageProgress.CompletedById = userId;
                    _context.TeamStageProgress.Update(teamStageProgress);
                }
            }

            var teamTaskProgress = await _context.TeamTaskProgress
                .FirstOrDefaultAsync(ttp => ttp.TaskId == taskId && ttp.TeamId == teamId);

            if (teamTaskProgress == null)
            {
                teamTaskProgress = new TeamTaskProgress
                {
                    TaskId = taskId,
                    TeamId = teamId,
                    Status = ProjectTaskStatus.Completed,
                    CompletedDate = DateTime.UtcNow,
                    CompletedById = userId
                };
                _context.TeamTaskProgress.Add(teamTaskProgress);
            }
            else
            {
                teamTaskProgress.Status = ProjectTaskStatus.Completed;
                teamTaskProgress.CompletedDate = DateTime.UtcNow;
                teamTaskProgress.CompletedById = userId;
                _context.TeamTaskProgress.Update(teamTaskProgress);
            }

            await _context.SaveChangesAsync();
            return new { Success = true, Message = $"Task and all its stages completed for team {team.Name}" };
        }
        
        private async Task<object> CompleteSingleStageForTeamAsync(Guid stageId, Guid teamId, Guid userId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.Task)
                .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(s => s.Id == stageId);
    
            if (stage == null)
                throw new Exception("Stage not found");

            if (!await CanUserManageProjectsAsync(userId, stage.Task.Project.SubjectId))
                throw new Exception("Only teachers and assists can complete stages");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null || team.SubjectId != stage.Task.Project.SubjectId)
                throw new Exception("Team not found or not in the same subject");

            var teamStageProgress = await _context.TeamStageProgress
                .FirstOrDefaultAsync(tsp => tsp.StageId == stageId && tsp.TeamId == teamId);

            if (teamStageProgress == null)
            {
                teamStageProgress = new TeamStageProgress
                {
                    StageId = stageId,
                    TeamId = teamId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    CompletedById = userId
                };
                _context.TeamStageProgress.Add(teamStageProgress);
            }
            else
            {
                teamStageProgress.IsCompleted = true;
                teamStageProgress.CompletedAt = DateTime.UtcNow;
                teamStageProgress.CompletedById = userId;
                _context.TeamStageProgress.Update(teamStageProgress);
            }

            await _context.SaveChangesAsync();
            await CheckAndUpdateTaskCompletionForTeamAsync(stage.TaskId, teamId);
            return new { Success = true, Message = $"Stage completed for team {team.Name}" };
        }
        
        private async Task<object> UncompleteSingleTaskForTeamAsync(Guid taskId, Guid teamId, Guid userId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);
    
            if (task == null)
                throw new Exception("Task not found");

            if (!await CanUserManageProjectsAsync(userId, task.Project.SubjectId))
                throw new Exception("Only teachers and assists can uncomplete tasks");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null || team.SubjectId != task.Project.SubjectId)
                throw new Exception("Team not found or not in the same subject");

            var teamTaskProgress = await _context.TeamTaskProgress
                .FirstOrDefaultAsync(ttp => ttp.TaskId == taskId && ttp.TeamId == teamId);

            if (teamTaskProgress == null)
                throw new Exception("Task progress not found for this team");

            if (teamTaskProgress.Status != ProjectTaskStatus.Completed)
                throw new Exception("Task is not completed, cannot uncomplete");

            teamTaskProgress.Status = ProjectTaskStatus.InProgress;
            teamTaskProgress.CompletedDate = null;
            teamTaskProgress.CompletedById = null;

            _context.TeamTaskProgress.Update(teamTaskProgress);
            await _context.SaveChangesAsync();
            return new { Success = true, Message = $"Task uncompleted for team {team.Name}" };
        }
        
        private async Task<object> UncompleteSingleStageForTeamAsync(Guid stageId, Guid teamId, Guid userId)
        {
            var stage = await _context.TaskStages
                .Include(s => s.Task)
                .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(s => s.Id == stageId);
    
            if (stage == null)
                throw new Exception("Stage not found");

            if (!await CanUserManageProjectsAsync(userId, stage.Task.Project.SubjectId))
                throw new Exception("Only teachers and assists can uncomplete stages");

            var teamStageProgress = await _context.TeamStageProgress
                .FirstOrDefaultAsync(tsp => tsp.StageId == stageId && tsp.TeamId == teamId);

            if (teamStageProgress == null)
                throw new Exception("Stage progress not found for this team");

            teamStageProgress.IsCompleted = false;
            teamStageProgress.CompletedAt = null;
            teamStageProgress.CompletedById = null;

            _context.TeamStageProgress.Update(teamStageProgress);
            await _context.SaveChangesAsync();
            await CheckAndUpdateTaskCompletionForTeamAsync(stage.TaskId, teamId);
            return new { Success = true, Message = $"Stage uncompleted for team" };
        }
    }
}