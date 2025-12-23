using System.ComponentModel.DataAnnotations;

namespace ReverseGanttChart.Models.Project
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Date < DateTime.UtcNow.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date must be today or in the future");
                }
            }
            return ValidationResult.Success;
        }
    }
}