using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Models.Project
{
    public class TeamTaskProgress
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public ProjectTask Task { get; set; }
        public Guid TeamId { get; set; }
        public Team.Team Team { get; set; }
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Pending;
        public DateTime? CompletedDate { get; set; }
        public Guid? CompletedById { get; set; }
        public User CompletedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}