namespace ReverseGanttChart.Models.Project
{
    public class ProjectTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Pending;
        public ProjectTaskPriority Priority { get; set; } = ProjectTaskPriority.Medium;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public Guid? ParentTaskId { get; set; }
        public ProjectTask ParentTask { get; set; }
        public ICollection<ProjectTask> Subtasks { get; set; } = new List<ProjectTask>();
        public ICollection<TaskStage> Stages { get; set; } = new List<TaskStage>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ProjectTaskStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2
    }

    public enum ProjectTaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}