namespace ReverseGanttChart.Services.Role;

public interface IRolesService
{
    Task<string> GrantAssistRoleAsync(Guid userId);
}