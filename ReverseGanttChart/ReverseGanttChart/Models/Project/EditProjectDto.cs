using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    public class EditProjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }
    }
}