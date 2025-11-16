using System.ComponentModel.DataAnnotations;
using ReverseGanttChart.Models.Validation;

namespace ReverseGanttChart.Models;

public class RegisterDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 50 characters.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [PasswordValidation] 
    public string Password { get; set; }
    
    [Required(ErrorMessage = "IsStudent field is required.")]
    public bool IsStudent { get; set; }
}