using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    public class EditStageDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public float EstimatedEffort { get; set; }
    }
}