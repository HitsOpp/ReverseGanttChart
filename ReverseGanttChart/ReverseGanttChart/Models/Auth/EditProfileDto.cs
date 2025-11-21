using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models;

public class EditProfileDto
{
    [Required]
    public string FullName { get; set; }
}