namespace ReverseGanttChart.Models
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedByName { get; set; }
        public int MemberCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }
}