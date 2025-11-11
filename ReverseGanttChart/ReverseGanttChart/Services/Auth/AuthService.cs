using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Data;
using ReverseGanttChart.Models;
using ReverseGanttChart.Services.JWT;

namespace ReverseGanttChart.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<string> Register(RegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("User already exists.");
        
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return _jwtService.GenerateToken(user);
    }

    public async Task<string> Login(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");
        
        return _jwtService.GenerateToken(user);
    }
    
}