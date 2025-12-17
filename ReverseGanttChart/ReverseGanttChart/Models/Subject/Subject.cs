namespace ReverseGanttChart.Models;

public class Subject
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; } = "blue";
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public ICollection<UserSubject> UserSubjects { get; set; } = new List<UserSubject>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}