using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services.Team
{
    public interface ITeamService
    {
        Task<IActionResult> CreateTeamAsync(Guid subjectId, CreateTeamDto request, Guid userId);
        Task<IActionResult> EditTeamAsync(Guid teamId, EditTeamDto request, Guid userId);
        Task<IActionResult> DeleteTeamAsync(Guid teamId, Guid userId);
        Task<IActionResult> GetSubjectTeamsAsync(Guid subjectId);
        Task<IActionResult> GetTeamAsync(Guid teamId);
        Task<IActionResult> JoinTeamAsync(Guid teamId, JoinTeamDto request, Guid userId);
        Task<IActionResult> LeaveTeamAsync(Guid teamId, Guid userId);
        Task<IActionResult> RemoveTeamMemberAsync(Guid teamId, Guid memberUserId, Guid currentUserId);
        Task<IActionResult> GetUserTeamInSubjectAsync(Guid subjectId, Guid userId);
        Task<IActionResult> GetAllUserTeamsAsync(Guid userId);
        Task<IActionResult> CanUserJoinTeamAsync(Guid teamId, Guid userId);
        Task<IActionResult> UpdateTechStackAsync(Guid teamId, UpdateTechStackDto request, Guid userId);
    }
}