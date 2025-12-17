namespace ReverseGanttChart.Models;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsTeacher { get; set; } = false;
    public ICollection<UserSubject> UserSubjects { get; set; } = new List<UserSubject>();
    public ICollection<Subject> CreatedSubjects { get; set; } = new List<Subject>();
}