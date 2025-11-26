using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;
using ReverseGanttChart.Models.Team;

namespace ReverseGanttChart.Services.Team
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetUserTeamInSubjectAsync(Guid subjectId, Guid userId)
        {
            var userTeam = await _context.TeamMembers
                .Include(tm => tm.Team)
                .ThenInclude(t => t.Subject)
                .Include(tm => tm.Team)
                .ThenInclude(t => t.CreatedBy)
                .Include(tm => tm.Team)
                .ThenInclude(t => t.TeamMembers)
                .ThenInclude(tm => tm.User)
                .Where(tm => tm.UserId == userId && tm.Team.SubjectId == subjectId)
                .Select(tm => new TeamDto
                {
                    Id = tm.Team.Id,
                    Name = tm.Team.Name,
                    Description = tm.Team.Description,
                    CreatedByName = tm.Team.CreatedBy.FullName,
                    MemberCount = tm.Team.TeamMembers.Count,
                    CreatedAt = tm.Team.CreatedAt,
                    Members = tm.Team.TeamMembers.Select(member => new TeamMemberDto
                    {
                        UserId = member.UserId,
                        FullName = member.User.FullName,
                        Email = member.User.Email,
                        TechStack = member.TechStack,
                        JoinedAt = member.JoinedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (userTeam == null)
                return new NotFoundObjectResult("User is not in any team in this subject");

            return new OkObjectResult(userTeam);
        }

        public async Task<IActionResult> GetAllUserTeamsAsync(Guid userId)
        {
            var userTeams = await _context.TeamMembers
                .Include(tm => tm.Team)
                .ThenInclude(t => t.Subject)
                .Include(tm => tm.Team)
                .ThenInclude(t => t.CreatedBy)
                .Where(tm => tm.UserId == userId)
                .Select(tm => new
                {
                    TeamId = tm.Team.Id,
                    TeamName = tm.Team.Name,
                    SubjectId = tm.Team.SubjectId,
                    SubjectName = tm.Team.Subject.Name,
                    TechStack = tm.TechStack,
                    JoinedAt = tm.JoinedAt
                })
                .ToListAsync();

            return new OkObjectResult(userTeams);
        }

        public async Task<IActionResult> CanUserJoinTeamAsync(Guid teamId, Guid userId)
        {
            var team = await _context.Teams
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new NotFoundObjectResult("Team not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found");

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == team.SubjectId);

            var canJoinResult = await CanUserJoinTeamInternalAsync(userId, team.SubjectId, user.IsTeacher, userSubject?.Role);
            
            return new OkObjectResult(new 
            { 
                canJoin = canJoinResult.canJoin,
                message = canJoinResult.message,
                currentTeamsInSubject = await GetUserTeamsCountInSubjectAsync(userId, team.SubjectId)
            });
        }

        private async Task<(bool canJoin, string message)> CanUserJoinTeamInternalAsync(Guid userId, Guid subjectId, bool isGlobalTeacher, SubjectRole? subjectRole)
        {
            var userTeamsInSubject = await _context.TeamMembers
                .Include(tm => tm.Team)
                .Where(tm => tm.UserId == userId && tm.Team.SubjectId == subjectId)
                .ToListAsync();

            if (isGlobalTeacher)
            {
                return (true, "Teacher can join multiple teams");
            }

            if (subjectRole.HasValue)
            {
                switch (subjectRole.Value)
                {
                    case SubjectRole.Teacher:
                        return (true, "Subject teacher can join multiple teams");
                        
                    case SubjectRole.Assist:
                        return (true, "Assist can join multiple teams in the same subject");
                        
                    case SubjectRole.Student:
                        if (userTeamsInSubject.Any())
                        {
                            return (false, "Student cannot be in multiple teams in the same subject");
                        }
                        return (true, "Student can join one team");
                        
                    default:
                        return (false, "Unknown user role");
                }
            }

            return (false, "User does not have a role in this subject");
        }

        private async Task<int> GetUserTeamsCountInSubjectAsync(Guid userId, Guid subjectId)
        {
            return await _context.TeamMembers
                .CountAsync(tm => tm.UserId == userId && tm.Team.SubjectId == subjectId);
        }

        public async Task<IActionResult> CreateTeamAsync(Guid subjectId, CreateTeamDto request, Guid userId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                return new NotFoundObjectResult("Subject not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found");

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == subjectId);

            if (userSubject == null && !user.IsTeacher)
                return new UnauthorizedObjectResult("User is not enrolled in this subject");

            var team = new Models.Team.Team
            {
                Name = request.Name,
                Description = request.Description,
                SubjectId = subjectId,
                CreatedById = userId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            var teamMember = new TeamMember
            {
                TeamId = team.Id,
                UserId = userId,
                TechStack = request.TechStack ?? "Not specified" 
            };

            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();

            var teamDto = await GetTeamDtoAsync(team.Id);
            return new OkObjectResult(teamDto);
        }

        public async Task<IActionResult> EditTeamAsync(Guid teamId, EditTeamDto request, Guid userId)
        {
            var team = await _context.Teams
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new NotFoundObjectResult("Team not found");

            var isCreator = team.CreatedById == userId;
            var isTeacher = await IsUserTeacherInSubjectAsync(userId, team.SubjectId);

            if (!isCreator && !isTeacher)
                return new UnauthorizedObjectResult("Only team creator or teacher can edit team");

            team.Name = request.Name;
            team.Description = request.Description;

            _context.Teams.Update(team);
            await _context.SaveChangesAsync();

            var teamDto = await GetTeamDtoAsync(team.Id);
            return new OkObjectResult(teamDto);
        }

        public async Task<IActionResult> DeleteTeamAsync(Guid teamId, Guid userId)
        {
            var team = await _context.Teams
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new NotFoundObjectResult("Team not found");

            var isCreator = team.CreatedById == userId;
            var isTeacher = await IsUserTeacherInSubjectAsync(userId, team.SubjectId);

            if (!isCreator && !isTeacher)
                return new UnauthorizedObjectResult("Only team creator or teacher can delete team");

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Team deleted successfully" });
        }

        public async Task<IActionResult> GetSubjectTeamsAsync(Guid subjectId)
        {
            var teams = await _context.Teams
                .Where(t => t.SubjectId == subjectId)
                .Include(t => t.CreatedBy)
                .Include(t => t.TeamMembers)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    CreatedByName = t.CreatedBy.FullName,
                    MemberCount = t.TeamMembers.Count,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new OkObjectResult(teams);
        }

        public async Task<IActionResult> GetTeamAsync(Guid teamId)
        {
            var teamDto = await GetTeamDtoAsync(teamId);
            if (teamDto == null)
                return new NotFoundObjectResult("Team not found");

            return new OkObjectResult(teamDto);
        }

        public async Task<IActionResult> JoinTeamAsync(Guid teamId, JoinTeamDto request, Guid userId)
        {
            var team = await _context.Teams
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new NotFoundObjectResult("Team not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found");

            var userSubject = await _context.UserSubjects
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SubjectId == team.SubjectId);

            if (userSubject == null && !user.IsTeacher)
                return new UnauthorizedObjectResult("User is not enrolled in this subject");

            var existingMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);

            if (existingMember != null)
                return new BadRequestObjectResult("User already in this team");

            var canJoinResult = await CanUserJoinTeamInternalAsync(userId, team.SubjectId, user.IsTeacher, userSubject?.Role);
            if (!canJoinResult.canJoin)
                return new BadRequestObjectResult(canJoinResult.message);

            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                TechStack = request.TechStack ?? "Not specified" 
            };

            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Successfully joined team" });
        }

        public async Task<IActionResult> LeaveTeamAsync(Guid teamId, Guid userId)
        {
            var teamMember = await _context.TeamMembers
                .Include(tm => tm.Team)
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (teamMember == null)
                return new NotFoundObjectResult("User is not a member of this team");

            if (teamMember.Team.CreatedById == userId)
            {
                var teamMembersCount = await _context.TeamMembers
                    .CountAsync(tm => tm.TeamId == teamId);

                if (teamMembersCount == 1)
                {
                    _context.Teams.Remove(teamMember.Team);
                    await _context.SaveChangesAsync();
                    return new OkObjectResult(new { message = "Team deleted successfully as creator was the only member" });
                }
                else
                {
                    return new BadRequestObjectResult("Team creator cannot leave the team. Transfer ownership or delete team.");
                }
            }

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Successfully left team" });
        }

        public async Task<IActionResult> RemoveTeamMemberAsync(Guid teamId, Guid memberUserId, Guid currentUserId)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new NotFoundObjectResult("Team not found");

            var isCreator = team.CreatedById == currentUserId;
            var isTeacher = await IsUserTeacherInSubjectAsync(currentUserId, team.SubjectId);

            if (!isCreator && !isTeacher)
                return new UnauthorizedObjectResult("Only team creator or teacher can remove members");

            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == memberUserId);

            if (teamMember == null)
                return new NotFoundObjectResult("Team member not found");

            if (memberUserId == team.CreatedById)
                return new BadRequestObjectResult("Cannot remove team creator");

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Team member removed successfully" });
        }

        private async Task<TeamDto> GetTeamDtoAsync(Guid teamId)
        {
            var team = await _context.Teams
                .Include(t => t.CreatedBy)
                .Include(t => t.TeamMembers)
                    .ThenInclude(tm => tm.User)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return null;

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                CreatedByName = team.CreatedBy.FullName,
                MemberCount = team.TeamMembers.Count,
                CreatedAt = team.CreatedAt,
                Members = team.TeamMembers.Select(tm => new TeamMemberDto
                {
                    UserId = tm.UserId,
                    FullName = tm.User.FullName,
                    Email = tm.User.Email,
                    TechStack = tm.TechStack,
                    JoinedAt = tm.JoinedAt
                }).ToList()
            };
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
        
        public async Task<IActionResult> UpdateTechStackAsync(Guid teamId, UpdateTechStackDto request, Guid userId)
        {
            var teamMember = await _context.TeamMembers
                .Include(tm => tm.Team)
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (teamMember == null)
                return new NotFoundObjectResult("User is not a member of this team");

            teamMember.TechStack = request.TechStack;
            _context.TeamMembers.Update(teamMember);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Tech stack updated successfully" });
        }
    }
}