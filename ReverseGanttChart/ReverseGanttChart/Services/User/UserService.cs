using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;

namespace ReverseGanttChart.Services.User;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .Select(u => new
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            })
            .ToListAsync();
        
        return new OkObjectResult(users);
    }

    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Role) 
            .SingleOrDefaultAsync(u => u.Id == userId);
    
        if (user == null)
        {
            return new NotFoundObjectResult("User not found.");
        }
    
        if (user.Role == null)
        {
            return new NotFoundObjectResult("User role not found.");
        }
    
        var roles = new
        {
            IsStudent = user.Role.IsStudent,
            IsTeacher = user.Role.IsTeacher,
            IsAssistant = user.Role.IsAssist
        };

        return new OkObjectResult(roles);
    }
}