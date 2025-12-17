using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services;

public interface IAuthService
{
    Task<string> Register(RegisterDto request);
    Task<string> Login(LoginDto loginDto);
    Task<UserProfileDto> GetProfile(Guid userId);
    Task<UserProfileDto> EditProfile(Guid userId, EditProfileDto request);
}