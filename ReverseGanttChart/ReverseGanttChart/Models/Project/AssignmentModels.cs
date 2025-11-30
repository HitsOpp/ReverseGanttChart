namespace ReverseGanttChart.Models.Project
{
    public class TaskAssignment
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public ProjectTask ProjectTask { get; set; }
        public Guid TeamId { get; set; }
        public Team.Team Team { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }

    public class StageAssignment
    {
        public Guid Id { get; set; }
        public Guid StageId { get; set; }
        public TaskStage Stage { get; set; }
        public Guid TeamId { get; set; }
        public Team.Team Team { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}