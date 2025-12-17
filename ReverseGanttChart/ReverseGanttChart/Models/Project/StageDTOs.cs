using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Models.Project
{
    public class CreateStageDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float EstimatedEffort { get; set; }
    }

    public class StageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float EstimatedEffort { get; set; }
        public List<TeamStageProgressDto> TeamProgress { get; set; } = new List<TeamStageProgressDto>();
        public DateTime CreatedAt { get; set; }
    }

    public class TeamStageProgressDto
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string CompletedByName { get; set; }
    }
}