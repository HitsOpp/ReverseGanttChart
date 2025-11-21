namespace ReverseGanttChart.Models.Team;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}