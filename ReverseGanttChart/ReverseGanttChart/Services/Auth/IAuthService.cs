using ReverseGanttChart.Models;

namespace ReverseGanttChart.Services;

public interface IAuthService
{
    Task<string> Register(RegisterDto request);
    Task<string> Login(LoginDto loginDto);
}