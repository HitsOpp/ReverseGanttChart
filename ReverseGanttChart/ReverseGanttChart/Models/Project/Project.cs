using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

namespace ReverseGanttChart.Models.Project
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    
}