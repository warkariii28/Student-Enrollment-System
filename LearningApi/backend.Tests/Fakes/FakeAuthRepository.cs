using BrightPath.DTOs;
using BrightPath.Models;
using BrightPath.Repositories;

namespace backend.Tests.Fakes;

public class FakeAuthRepository : IAuthRepository
{
    public User? UserByEmail { get; set; }
    public User? UserById { get; set; }
    public RefreshToken? RefreshTokenToReturn { get; set; }
    public bool UpdateEmailResult { get; set; } = true;
    public bool UpdateRoleResult { get; set; } = true;
    public int AdminCount { get; set; } = 2;

    public User? RegisteredUser { get; private set; }
    public int SavedRefreshTokenUserId { get; private set; }
    public string? SavedRefreshTokenHash { get; private set; }
    public string? RevokedRefreshTokenHash { get; private set; }
    public int RevokedActiveTokensUserId { get; private set; }
    public bool CleanupExpiredRefreshTokensCalled { get; private set; }
    public int UpdatedEmailUserId { get; private set; }
    public string? UpdatedEmail { get; private set; }
    public int UpdatedRoleUserId { get; private set; }
    public string? UpdatedRole { get; private set; }

    public void Register(User user)
    {
        RegisteredUser = user;
    }

    public User? GetByEmail(string email)
    {
        return UserByEmail;
    }

    public User? GetById(int userId)
    {
        return UserById;
    }

    public bool UpdateEmail(int userId, string email)
    {
        UpdatedEmailUserId = userId;
        UpdatedEmail = email;
        return UpdateEmailResult;
    }

    public bool UpdateRole(int userId, string role)
    {
        UpdatedRoleUserId = userId;
        UpdatedRole = role;
        return UpdateRoleResult;
    }

    public void SaveRefreshToken(int userId, string token)
    {
        SavedRefreshTokenUserId = userId;
        SavedRefreshTokenHash = token;
    }

    public RefreshToken? GetRefreshToken(string token)
    {
        return RefreshTokenToReturn;
    }

    public void RevokeRefreshToken(string token)
    {
        RevokedRefreshTokenHash = token;
    }

    public void CleanupExpiredRefreshTokens()
    {
        CleanupExpiredRefreshTokensCalled = true;
    }

    public void RevokeActiveRefreshTokensForUser(int userId)
    {
        RevokedActiveTokensUserId = userId;
    }

    public List<UserDto> GetAllUsers()
    {
        return [];
    }

    public int CountAdmins()
    {
        return AdminCount;
    }
}