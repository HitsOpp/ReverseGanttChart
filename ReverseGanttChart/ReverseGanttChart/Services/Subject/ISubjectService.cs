using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services.Subject;

public interface ISubjectService
{
    Task<SubjectDto> CreateSubjectAsync(Guid teacherId, CreateSubjectDto request);
    Task<string> JoinSubjectAsync(Guid userId, Guid subjectId);
    Task<List<SubjectDto>> GetUserSubjectsAsync(Guid userId);
    Task<List<SubjectDto>> GetAllSubjectsAsync();
    Task<string> GrantAssistRoleAsync(Guid subjectId, Guid userId, Guid currentUserId);
    Task<string> RevokeAssistRoleAsync(Guid subjectId, Guid userId, Guid currentUserId);
}