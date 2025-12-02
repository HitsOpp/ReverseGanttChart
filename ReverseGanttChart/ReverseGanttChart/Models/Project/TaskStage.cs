using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Models.Project
{
    namespace ReverseGanttChart.Models.Project
    {
        public class TaskStage
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public float EstimatedEffort { get; set; }
            public Guid TaskId { get; set; }
            public ProjectTask Task { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }
}