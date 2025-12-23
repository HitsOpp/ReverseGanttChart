using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        
        [Required]
        [FutureDate(ErrorMessage = "End date must be in the future")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }
    }
    

    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatedByName { get; set; }
        public int TaskCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}