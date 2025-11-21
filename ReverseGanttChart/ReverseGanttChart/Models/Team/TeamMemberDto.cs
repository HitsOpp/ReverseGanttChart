namespace ReverseGanttChart.Models
{
    public class TeamMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string TechStack { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}