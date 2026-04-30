using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Services;

public interface IAuthService
{
    void Register(User user, string password);

    AuthResultDto Login(LoginDto dto);

    void UpdateEmail(int userId, string email);

    void UpdateRole(int userId, string role);

    RefreshToken? ValidateRefreshToken(string token);
    User GetUserById(int userId);
    string GenerateToken(User user);
    string RotateRefreshToken(string oldToken);
}