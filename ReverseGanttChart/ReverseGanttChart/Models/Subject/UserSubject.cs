namespace ReverseGanttChart.Models;

public class UserSubject
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public SubjectRole Role { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public enum SubjectRole
{
    Student = 0,
    Assist = 1,
    Teacher = 2
}