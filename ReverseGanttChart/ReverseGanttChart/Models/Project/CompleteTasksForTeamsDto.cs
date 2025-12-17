using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    public class CompleteTasksForTeamsDto
    {
        [Required]
        public List<Guid> TeamIds { get; set; } = new List<Guid>();
        
        [Required]
        public List<Guid> TaskIds { get; set; } = new List<Guid>();
    }

    public class CompleteStagesForTeamsDto
    {
        [Required]
        public List<Guid> TeamIds { get; set; } = new List<Guid>();
        
        [Required]
        public List<Guid> StageIds { get; set; } = new List<Guid>();
    }

    public class UncompleteTasksForTeamsDto
    {
        [Required]
        public List<Guid> TeamIds { get; set; } = new List<Guid>();
        
        [Required]
        public List<Guid> TaskIds { get; set; } = new List<Guid>();
    }

    public class UncompleteStagesForTeamsDto
    {
        [Required]
        public List<Guid> TeamIds { get; set; } = new List<Guid>();
        
        [Required]
        public List<Guid> StageIds { get; set; } = new List<Guid>();
    }
}