namespace ReverseGanttChart.Models
{
    public class SubjectInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string CreatorName { get; set; } 
        public int StudentCount { get; set; }
        public int AssistCount { get; set; }
        public int TeacherCount { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}