namespace ReverseGanttChart.Models;

public class Role
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool IsTeacher { get; set; } = false;
    public bool IsStudent { get; set; } = false;
    public bool IsAssist { get; set; } = false;

}