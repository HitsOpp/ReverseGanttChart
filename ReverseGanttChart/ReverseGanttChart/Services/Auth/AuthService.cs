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
    
        var user = new Models.User
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
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            return _jwtService.GenerateToken(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error for {loginDto.Email}: {ex.Message}");
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
    }
    public async Task<UserProfileDto> GetProfile(Guid userId)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
            throw new KeyNotFoundException("User not found.");
        
        return new UserProfileDto
        {
            FullName = user.FullName,
            Email = user.Email,
        };
    }

    public async Task<UserProfileDto> EditProfile(Guid userId, EditProfileDto request)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
            throw new KeyNotFoundException("User not found.");
        
        user.FullName = request.FullName;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        return new UserProfileDto
        {
            FullName = user.FullName,
            Email = user.Email,
        };
    }
}