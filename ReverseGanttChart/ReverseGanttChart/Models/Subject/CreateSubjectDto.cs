using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models;

public class CreateSubjectDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }
}