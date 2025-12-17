using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services.Subject
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;

        public SubjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubjectDto> CreateSubjectAsync(Guid teacherId, CreateSubjectDto request)
        {
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null || !teacher.IsTeacher)
                throw new UnauthorizedAccessException("Only teachers can create subjects");

            var subject = new Models.Subject
            {
                Name = request.Name,
                Description = request.Description,
                Color = request.Color,
                CreatedById = teacherId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return await GetSubjectDtoAsync(subject.Id, teacherId);
        }

        public async Task<string> JoinSubjectAsync(Guid userId, Guid subjectId)
        {
            var subject = await _context.Subjects
                .Include(s => s.CreatedBy)
                .FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
                throw new KeyNotFoundException("Subject not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");
    
            if (subject.CreatedById == userId)
            {
                return "Subject creator is automatically a teacher and cannot join as a participant";
            }
    
            var existingUserSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId);
    
            if (existingUserSubject != null)
            {
                return $"User already joined this subject with role: {existingUserSubject.Role}";
            }
    
            if (user.IsTeacher)
            {
                var userSubject = new UserSubject
                {
                    UserId = userId,
                    SubjectId = subjectId,
                    Role = SubjectRole.Teacher
                };

                _context.UserSubjects.Add(userSubject);
                await _context.SaveChangesAsync();

                return $"Teacher {user.FullName} successfully joined subject: {subject.Name} as Teacher";
            }
            else
            {
                var userSubject = new UserSubject
                {
                    UserId = userId,
                    SubjectId = subjectId,
                    Role = SubjectRole.Student
                };

                _context.UserSubjects.Add(userSubject);
                await _context.SaveChangesAsync();

                return $"Successfully joined subject: {subject.Name} as Student";
            }
        }

        public async Task<string> GrantAssistRoleAsync(Guid subjectId, Guid userId, Guid currentUserId)
        {
            var currentUserIsTeacher = await IsUserTeacherInSubjectAsync(currentUserId, subjectId);
            if (!currentUserIsTeacher)
                throw new UnauthorizedAccessException("Only teachers can grant assist role");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            
            if (user.IsTeacher)
                throw new InvalidOperationException("Teachers cannot be assigned as assists");

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId);

            if (userSubject == null)
                throw new KeyNotFoundException("User is not enrolled in this subject");
            
            userSubject.Role = SubjectRole.Assist;
            await _context.SaveChangesAsync();

            return $"User {user.FullName} granted assist role in subject (no longer Student)";
        }

        public async Task<string> RevokeAssistRoleAsync(Guid subjectId, Guid userId, Guid currentUserId)
        {
            var currentUserIsTeacher = await IsUserTeacherInSubjectAsync(currentUserId, subjectId);
            if (!currentUserIsTeacher)
                throw new UnauthorizedAccessException("Only teachers can revoke assist role");

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId);

            if (userSubject == null)
                throw new KeyNotFoundException("User is not enrolled in this subject");

            userSubject.Role = SubjectRole.Student;
            await _context.SaveChangesAsync();

            return $"User assist role revoked in subject, now has Student role";
        }

        public async Task<List<SubjectDto>> GetUserSubjectsAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            
            List<SubjectDto> subjects;

            if (user.IsTeacher)
            {
                subjects = await _context.UserSubjects
                    .Where(us => us.UserId == userId && us.Role == SubjectRole.Teacher)
                    .Include(us => us.Subject)
                        .ThenInclude(s => s.CreatedBy)
                    .Include(us => us.Subject)
                        .ThenInclude(s => s.UserSubjects)
                    .Select(us => new SubjectDto
                    {
                        Id = us.Subject.Id,
                        Name = us.Subject.Name,
                        Description = us.Subject.Description,
                        Color = us.Subject.Color,
                        CreatorName = us.Subject.CreatedBy.FullName,
                        CurrentUserRole = "Teacher",
                        StudentCount = us.Subject.UserSubjects.Count(us => us.Role == SubjectRole.Student),
                        AssistCount = us.Subject.UserSubjects.Count(us => us.Role == SubjectRole.Assist),
                        TeacherCount = us.Subject.UserSubjects.Count(us => us.Role == SubjectRole.Teacher) + 1,
                        CreatedAt = us.Subject.CreatedAt
                    })
                    .ToListAsync();
                
                var createdSubjects = await _context.Subjects
                    .Where(s => s.CreatedById == userId)
                    .Include(s => s.CreatedBy)
                    .Include(s => s.UserSubjects)
                    .Select(s => new SubjectDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Color = s.Color,
                        CreatorName = s.CreatedBy.FullName,
                        CurrentUserRole = "Teacher",
                        StudentCount = s.UserSubjects.Count(us => us.Role == SubjectRole.Student),
                        AssistCount = s.UserSubjects.Count(us => us.Role == SubjectRole.Assist),
                        TeacherCount = s.UserSubjects.Count(us => us.Role == SubjectRole.Teacher) + 1,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();
                
                var allSubjects = subjects.Union(createdSubjects)
                    .GroupBy(s => s.Id)
                    .Select(g => g.First())
                    .ToList();

                subjects = allSubjects;
            }
            else
            {
                subjects = await _context.UserSubjects
                    .Where(us => us.UserId == userId)
                    .Include(us => us.Subject)
                        .ThenInclude(s => s.CreatedBy)
                    .Include(us => us.Subject)
                        .ThenInclude(s => s.UserSubjects)
                    .Select(us => new SubjectDto
                    {
                        Id = us.Subject.Id,
                        Name = us.Subject.Name,
                        Description = us.Subject.Description,
                        Color = us.Subject.Color,
                        CreatorName = us.Subject.CreatedBy.FullName,
                        CurrentUserRole = us.Role.ToString(),
                        StudentCount = us.Subject.UserSubjects.Count(us => us.Role == SubjectRole.Student),
                        AssistCount = us.Subject.UserSubjects.Count(us => us.Role == SubjectRole.Assist),
                        TeacherCount = us.Subject.UserSubjects.Count(us => us.Role == SubjectRole.Teacher) + 1,
                        CreatedAt = us.Subject.CreatedAt
                    })
                    .ToListAsync();
            }

            return subjects;
        }

        public async Task<List<SubjectInfoDto>> GetAllSubjectsAsync()
        {
            var subjects = await _context.Subjects
                .Include(s => s.CreatedBy)
                .Include(s => s.UserSubjects)
                .Select(s => new SubjectInfoDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Color = s.Color,
                    CreatorName = s.CreatedBy.FullName,
                    StudentCount = s.UserSubjects.Count(us => us.Role == SubjectRole.Student),
                    AssistCount = s.UserSubjects.Count(us => us.Role == SubjectRole.Assist),
                    TeacherCount = s.UserSubjects.Count(us => us.Role == SubjectRole.Teacher) + 1,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return subjects;
        }

        public async Task<IActionResult> EditSubjectAsync(Guid subjectId, EditSubjectDto request, Guid userId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException("Subject not found");
    
            if (subject.CreatedById != userId)
                throw new UnauthorizedAccessException("Only subject creator can edit subject");

            subject.Name = request.Name;
            subject.Description = request.Description;
            subject.Color = request.Color;

            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();

            var subjectDto = await GetSubjectDtoAsync(subject.Id, userId);
            return new OkObjectResult(subjectDto);
        }

        public async Task<IActionResult> DeleteSubjectAsync(Guid subjectId, Guid userId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException("Subject not found");
            
            if (subject.CreatedById != userId)
                throw new UnauthorizedAccessException("Only subject creator can delete subject");

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Subject deleted successfully" });
        }

        public async Task<IActionResult> GetSubjectByIdAsync(Guid subjectId)
        {
            var subject = await _context.Subjects
                .Include(s => s.CreatedBy)
                .Include(s => s.UserSubjects)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
                return new NotFoundObjectResult("Subject not found");

            var subjectDto = new SubjectInfoDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                Color = subject.Color,
                CreatorName = subject.CreatedBy.FullName,
                StudentCount = subject.UserSubjects.Count(us => us.Role == SubjectRole.Student),
                AssistCount = subject.UserSubjects.Count(us => us.Role == SubjectRole.Assist),
                TeacherCount = subject.UserSubjects.Count(us => us.Role == SubjectRole.Teacher) + 1,
                CreatedAt = subject.CreatedAt
            };

            return new OkObjectResult(subjectDto);
        }

        private async Task<SubjectDto> GetSubjectDtoAsync(Guid subjectId, Guid? currentUserId = null)
        {
            var subject = await _context.Subjects
                .Include(s => s.CreatedBy)
                .Include(s => s.UserSubjects)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
                throw new KeyNotFoundException("Subject not found");

            var subjectDto = new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                Color = subject.Color,
                CreatorName = subject.CreatedBy.FullName,
                StudentCount = subject.UserSubjects.Count(us => us.Role == SubjectRole.Student),
                AssistCount = subject.UserSubjects.Count(us => us.Role == SubjectRole.Assist),
                TeacherCount = subject.UserSubjects.Count(us => us.Role == SubjectRole.Teacher) + 1,
                CreatedAt = subject.CreatedAt
            };

            if (currentUserId.HasValue)
            {
                var user = await _context.Users.FindAsync(currentUserId.Value);
                if (user != null && user.IsTeacher)
                {
                    var isTeacherInSubject = await IsUserTeacherInSubjectAsync(currentUserId.Value, subjectId);
                    subjectDto.CurrentUserRole = isTeacherInSubject ? "Teacher" : "NotEnrolled";
                }
                else
                {
                    var userSubject = await _context.UserSubjects
                        .FirstOrDefaultAsync(us => us.UserId == currentUserId.Value && us.SubjectId == subjectId);
                    
                    subjectDto.CurrentUserRole = userSubject?.Role.ToString() ?? "NotEnrolled";
                }
            }

            return subjectDto;
        }

        private async Task<bool> IsUserTeacherInSubjectAsync(Guid userId, Guid subjectId)
        {
            var isCreator = await _context.Subjects
                .AnyAsync(s => s.Id == subjectId && s.CreatedById == userId);

            if (isCreator)
                return true;

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId && us.Role == SubjectRole.Teacher);

            return userSubject != null;
        }
                public async Task<IActionResult> GetSubjectStudentsAsync(Guid subjectId)
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
                    JoinedAt = us.JoinedAt
                })
                .ToListAsync();

            teachers.AddRange(joinedTeachers);

            return new OkObjectResult(teachers);
        }
        public async Task<IActionResult> GetUserRoleInSubjectAsync(Guid subjectId, Guid userId)
        {
            var subject = await _context.Subjects
                .Include(s => s.CreatedBy)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
                return new NotFoundObjectResult(new { message = "Subject not found" });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult(new { message = "User not found" });

            if (subject.CreatedById == userId)
            {
                return new OkObjectResult(new
                {
                    SubjectId = subjectId,
                    SubjectName = subject.Name,
                    UserId = userId,
                    UserName = user.FullName,
                    Role = "Creator",
                    RoleCode = SubjectRole.Teacher,
                    IsTeacher = true,
                    CanManage = true
                });
            }

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId);

            if (userSubject == null)
            {
                return new OkObjectResult(new
                {
                    SubjectId = subjectId,
                    SubjectName = subject.Name,
                    UserId = userId,
                    UserName = user.FullName,
                    Role = "NotEnrolled",
                    RoleCode = (SubjectRole?)null,
                    IsTeacher = false,
                    CanManage = false
                });
            }

            var isTeacher = userSubject.Role == SubjectRole.Teacher;
            var canManage = isTeacher || userSubject.Role == SubjectRole.Assist;

            return new OkObjectResult(new
            {
                Role = userSubject.Role.ToString()
            });
        }
    }
}