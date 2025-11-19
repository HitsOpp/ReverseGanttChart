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

    public async Task<IActionResult> GetUserSubjectsWithRolesAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return new NotFoundObjectResult("User not found");

        List<object> result;

        if (user.IsTeacher)
        {
            result = await _context.Subjects
                .Include(s => s.CreatedBy)
                .Select(s => new
                {
                    SubjectId = s.Id,
                    SubjectName = s.Name,
                    SubjectDescription = s.Description,
                    TeacherName = s.CreatedBy.FullName,
                    Role = "Teacher",
                    JoinedAt = s.CreatedAt
                })
                .ToListAsync<object>();
        }
        else
        {
            result = await _context.UserSubjects
                .Where(us => us.UserId == userId)
                .Include(us => us.Subject)
                    .ThenInclude(s => s.CreatedBy)
                .Select(us => new
                {
                    SubjectId = us.Subject.Id,
                    SubjectName = us.Subject.Name,
                    SubjectDescription = us.Subject.Description,
                    TeacherName = us.Subject.CreatedBy.FullName, 
                    Role = us.Role.ToString(),
                    JoinedAt = us.JoinedAt
                })
                .ToListAsync<object>();
        }

        return new OkObjectResult(result);
    }

    public async Task<IActionResult> GetSubjectStudentsAsync(Guid subjectId)
    {
        var participants = await _context.UserSubjects
            .Where(us => us.SubjectId == subjectId)
            .Include(us => us.User)
            .Select(us => new
            {
                UserId = us.UserId,
                FullName = us.User.FullName,
                Email = us.User.Email,
                IsGlobalTeacher = us.User.IsTeacher,
                Role = us.Role.ToString(),
                JoinedAt = us.JoinedAt
            })
            .ToListAsync();

        var subject = await _context.Subjects
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject != null && !participants.Any(p => p.UserId == subject.CreatedById))
        {
            participants.Insert(0, new
            {
                UserId = subject.CreatedById,
                FullName = subject.CreatedBy.FullName,
                Email = subject.CreatedBy.Email,
                IsGlobalTeacher = subject.CreatedBy.IsTeacher,
                Role = SubjectRole.Teacher.ToString(),
                JoinedAt = subject.CreatedAt
            });
        }

        return new OkObjectResult(participants);
    }

    public async Task<IActionResult> GetAllTeachersAsync()
    {
        var teachers = await _context.Users
            .Where(u => u.IsTeacher)
            .Select(u => new
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                CreatedSubjectsCount = u.CreatedSubjects.Count
            })
            .ToListAsync();

        return new OkObjectResult(teachers);
    }

    public async Task<IActionResult> GetSubjectStudentsOnlyAsync(Guid subjectId)
    {
        var students = await _context.UserSubjects
            .Where(us => us.SubjectId == subjectId && us.Role == SubjectRole.Student)
            .Include(us => us.User)
            .Select(us => new
            {
                UserId = us.UserId,
                FullName = us.User.FullName,
                Email = us.User.Email,
                JoinedAt = us.JoinedAt
            })
            .ToListAsync();

        return new OkObjectResult(students);
    }

    public async Task<IActionResult> GetSubjectAssistsAsync(Guid subjectId)
    {
        var assists = await _context.UserSubjects
            .Where(us => us.SubjectId == subjectId && us.Role == SubjectRole.Assist)
            .Include(us => us.User)
            .Select(us => new
            {
                UserId = us.UserId,
                FullName = us.User.FullName,
                Email = us.User.Email,
                JoinedAt = us.JoinedAt
            })
            .ToListAsync();

        return new OkObjectResult(assists);
    }
    
    public async Task<IActionResult> GetSubjectTeachersAsync(Guid subjectId)
    {
        var teachers = new List<object>();

        var subject = await _context.Subjects
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject != null)
        {
            teachers.Add(new
            {
                UserId = subject.CreatedById,
                FullName = subject.CreatedBy.FullName,
                Email = subject.CreatedBy.Email,
                IsCreator = true,
                IsGlobalTeacher = subject.CreatedBy.IsTeacher,
                JoinedAt = subject.CreatedAt
            });
        }

        var joinedTeachers = await _context.UserSubjects
            .Where(us => us.SubjectId == subjectId && us.Role == SubjectRole.Teacher && us.UserId != subject.CreatedById)
            .Include(us => us.User)
            .Select(us => new
            {
                UserId = us.UserId,
                FullName = us.User.FullName,
                Email = us.User.Email,
                IsCreator = false,
                IsGlobalTeacher = us.User.IsTeacher,
                JoinedAt = us.JoinedAt
            })
            .ToListAsync();

        teachers.AddRange(joinedTeachers);

        return new OkObjectResult(teachers);
    }
}