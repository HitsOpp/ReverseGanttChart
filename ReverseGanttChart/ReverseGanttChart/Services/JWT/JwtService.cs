using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services.JWT;

public class JwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _tokenLifetimeInHours;
    private readonly ApplicationDbContext _context; 

    public JwtService(IConfiguration config, ApplicationDbContext context) 
    {
        _secret = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Secret is not configured");
        _issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        _audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        _tokenLifetimeInHours = int.Parse(config["Jwt:TokenLifetimeInHours"] ?? "1");
        _context = context;
    }
    
    public string GenerateToken(Models.User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.FullName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.IsTeacher)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Teacher"));
        }

        if (!user.IsTeacher)
        {
            var userSubjects = _context.UserSubjects
                .Include(us => us.Subject)
                .Where(us => us.UserId == user.Id)
                .ToList();

            foreach (var userSubject in userSubjects)
            {
                claims.Add(new Claim($"Subject_{userSubject.SubjectId}_Role", userSubject.Role.ToString()));
            }

            if (userSubjects.Any(us => us.Role == SubjectRole.Student))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Student"));
            }
        
            if (userSubjects.Any(us => us.Role == SubjectRole.Assist))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Assist"));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(_tokenLifetimeInHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
