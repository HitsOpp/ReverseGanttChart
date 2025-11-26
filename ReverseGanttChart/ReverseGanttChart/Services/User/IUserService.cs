using Microsoft.AspNetCore.Mvc;

namespace ReverseGanttChart.Services.User;
public interface IUserService
{
    Task<IActionResult> GetAllUsersAsync();
    Task<IActionResult> GetUserProfileAsync(Guid userId);
}