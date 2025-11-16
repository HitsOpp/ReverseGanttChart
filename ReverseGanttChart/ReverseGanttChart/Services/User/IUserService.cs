using Microsoft.AspNetCore.Mvc;

namespace ReverseGanttChart.Services.User;

public interface IUserService
{
    Task<IActionResult> GetAllUsers(); 
    Task<IActionResult> GetUserRoles(Guid userId); 
}