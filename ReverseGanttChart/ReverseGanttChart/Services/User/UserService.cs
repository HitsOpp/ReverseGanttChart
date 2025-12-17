using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services.User;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Select(u => new
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                IsTeacher = u.IsTeacher,
                SubjectsCount = u.UserSubjects.Count,
                CreatedSubjectsCount = u.CreatedSubjects.Count
            })
            .ToListAsync();
        
        return new OkObjectResult(users);
    }

    public async Task<IActionResult> GetUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                IsTeacher = u.IsTeacher,
                TotalSubjects = u.UserSubjects.Count,
                TeachingSubjects = u.CreatedSubjects.Count
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return new NotFoundObjectResult("User not found");

        return new OkObjectResult(user);
    }
}