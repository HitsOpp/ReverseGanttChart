using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models;

public class JoinSubjectDto
{
    [Required]
    public Guid SubjectId { get; set; }
}