using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    namespace ReverseGanttChart.Models.Project
    {
        public class CreateTaskDto
        {
            [Required]
            [StringLength(100)]
            public string Name { get; set; }

            [StringLength(500)]
            public string Description { get; set; }

            [Required]
            [FutureDate(ErrorMessage = "Due date must be in the future")]
            public DateTime DueDate { get; set; }

            [Required]
            public ProjectTaskPriority Priority { get; set; }
        }
    }

    namespace ReverseGanttChart.Models.Project
    {
        public class ProjectTaskDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime DueDate { get; set; }
            public ProjectTaskPriority Priority { get; set; }
            public Guid? ParentTaskId { get; set; }
            public string ParentTaskName { get; set; }
            public List<TeamProgressDto> TeamProgress { get; set; } = new List<TeamProgressDto>();
            public DateTime CreatedAt { get; set; }
        }

        public class TeamProgressDto
        {
            public Guid TeamId { get; set; }
            public string TeamName { get; set; }
            public ProjectTaskStatus Status { get; set; }
            public DateTime? CompletedDate { get; set; }
            public string CompletedByName { get; set; }
            public int CompletedStageCount { get; set; }
            public int TotalStageCount { get; set; }
            public double Progress => TotalStageCount > 0 ? (double)CompletedStageCount / TotalStageCount * 100 : 0;
        }
    }
}