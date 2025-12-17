using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models
{
    public class EditTeamDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}