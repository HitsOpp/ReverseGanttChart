namespace ReverseGanttChart.Models.Project
{
    public class TaskDependency
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public ProjectTask ProjectTask { get; set; }
        public Guid DependsOnTaskId { get; set; }
        public ProjectTask DependsOnProjectTask { get; set; }
        public DependencyType Type { get; set; }
    }

    public class StageDependency
    {
        public Guid Id { get; set; }
        public Guid StageId { get; set; }
        public TaskStage Stage { get; set; }
        public Guid DependsOnStageId { get; set; }
        public TaskStage DependsOnStage { get; set; }
        public DependencyType Type { get; set; }
    }

    public enum DependencyType
    {
        FinishToStart = 0,
        StartToStart = 1,
        FinishToFinish = 2,
        StartToFinish = 3
    }
}