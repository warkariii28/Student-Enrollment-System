using BrightPath.DTOs;
using BrightPath.Models;

namespace BrightPath.Repositories;
using BrightPath.DTOs;

public interface IAuthRepository
{
    void Register(User user);
    User? GetByEmail(string email);
    User? GetById(int userId);
    bool UpdateEmail(int userId, string email);

    bool UpdateRole(int userId, string role);

    void SaveRefreshToken(int userId, string token);
    RefreshToken? GetRefreshToken(string token);
    void RevokeRefreshToken(string token);

    void CleanupExpiredRefreshTokens();
    void RevokeActiveRefreshTokensForUser(int userId);
    List<UserDto> GetAllUsers();
    int CountAdmins();
}