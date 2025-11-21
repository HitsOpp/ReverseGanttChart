namespace ReverseGanttChart.Models;
public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CreatorName { get; set; }
    public string CurrentUserRole { get; set; } 
    public int StudentCount { get; set; }
    public int AssistCount { get; set; }
    public DateTime CreatedAt { get; set; }
}