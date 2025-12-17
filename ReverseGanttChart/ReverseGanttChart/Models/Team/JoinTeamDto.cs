using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models
{
    public class JoinTeamDto
    {

        [StringLength(200)]
        public string TechStack { get; set; }
    }
}