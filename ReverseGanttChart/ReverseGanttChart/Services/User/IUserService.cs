using Microsoft.AspNetCore.Mvc;

namespace ReverseGanttChart.Services.User;
public interface IUserService
{
    Task<IActionResult> GetAllUsersAsync();
    Task<IActionResult> GetUserProfileAsync(Guid userId);
    Task<IActionResult> GetUserSubjectsWithRolesAsync(Guid userId);
    Task<IActionResult> GetSubjectStudentsAsync(Guid subjectId);
    Task<IActionResult> GetAllTeachersAsync();
    Task<IActionResult> GetSubjectStudentsOnlyAsync(Guid subjectId);
    Task<IActionResult> GetSubjectAssistsAsync(Guid subjectId);
    Task<IActionResult> GetSubjectTeachersAsync(Guid subjectId);
}