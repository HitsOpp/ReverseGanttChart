using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models
{
    public class JoinTeamDto
    {
        [Required]
        public Guid TeamId { get; set; }

        [StringLength(200)]
        public string TechStack { get; set; }
    }
}