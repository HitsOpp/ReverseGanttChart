namespace ReverseGanttChart.Models.Project
{
    public class CreateTaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public Guid? ParentTaskId { get; set; }
    }

    public class ProjectTaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public ProjectTaskPriority Priority { get; set; }
        public Guid? ParentTaskId { get; set; }
        public string ParentTaskName { get; set; }
        public int StageCount { get; set; }
        public int CompletedStageCount { get; set; }
        public double Progress => StageCount > 0 ? (double)CompletedStageCount / StageCount * 100 : 0;
        public DateTime CreatedAt { get; set; }
    }
}