using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models
{
    public class UpdateTechStackDto
    {
        [Required]
        [StringLength(200)]
        public string TechStack { get; set; }
    }
}