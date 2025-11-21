using Microsoft.AspNetCore.Mvc;
using ReverseGanttChart.Models;

public interface ISubjectService
{
    Task<SubjectDto> CreateSubjectAsync(Guid teacherId, CreateSubjectDto request);
    Task<string> JoinSubjectAsync(Guid userId, Guid subjectId);
    Task<List<SubjectDto>> GetUserSubjectsAsync(Guid userId);
    Task<List<SubjectDto>> GetAllSubjectsAsync();
    Task<string> GrantAssistRoleAsync(Guid subjectId, Guid userId, Guid currentUserId);
    Task<string> RevokeAssistRoleAsync(Guid subjectId, Guid userId, Guid currentUserId);
    Task<IActionResult> EditSubjectAsync(Guid subjectId, EditSubjectDto request, Guid userId); 
    Task<IActionResult> DeleteSubjectAsync(Guid subjectId, Guid userId);
}