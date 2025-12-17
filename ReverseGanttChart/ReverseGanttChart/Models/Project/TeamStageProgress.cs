using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Models.Project
{
    public class TeamStageProgress
    {
        public Guid Id { get; set; }
        public Guid StageId { get; set; }
        public TaskStage Stage { get; set; }
        public Guid TeamId { get; set; }
        public Team.Team Team { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public Guid? CompletedById { get; set; }
        public User CompletedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}