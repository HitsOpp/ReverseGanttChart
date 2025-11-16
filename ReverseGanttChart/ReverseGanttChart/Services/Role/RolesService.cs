using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;

namespace ReverseGanttChart.Services.Role;

public class RolesService : IRolesService
{
    private readonly ApplicationDbContext _context;

    public RolesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GrantAssistRoleAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        user.Role.IsAssist = true;
        await _context.SaveChangesAsync();

        return $"User {user.FullName} is now a Assist.";
    }
}