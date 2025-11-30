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
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string CompletedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}