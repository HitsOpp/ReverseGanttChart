namespace ReverseGanttChart.Models.Team;
public class TeamMember
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string TechStack { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}