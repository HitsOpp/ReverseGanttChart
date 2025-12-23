using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    public class EditTaskDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Due date must be in the future")]
        public DateTime DueDate { get; set; }

        [Required]
        public ProjectTaskPriority Priority { get; set; }
    }
}