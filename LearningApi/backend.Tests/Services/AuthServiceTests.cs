using BrightPath.Constants;
using BrightPath.DTOs;
using BrightPath.Exceptions;
using BrightPath.Models;
using BrightPath.Services;
using Microsoft.Extensions.Configuration;
using backend.Tests.Fakes;

namespace backend.Tests.Services;

public class AuthServiceTests
{
    private static IConfiguration CreateConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "this-is-a-test-secret-key-with-32-chars-min",
                ["Jwt:Issuer"] = "BrightPath.Tests",
                ["Jwt:Audience"] = "BrightPath.Tests.Users"
            })
            .Build();
    }

    [Fact]
    public void Register_HashesPasswordAndRegistersUser()
    {
        var repo = new FakeAuthRepository();
        var service = new AuthService(repo, CreateConfig());

        service.Register(new User
        {
            Username = "Asha",
            Email = "asha@example.com",
            Role = AppRoles.User
        }, "Password123");

        Assert.NotNull(repo.RegisteredUser);
        Assert.NotEqual("Password123", repo.RegisteredUser.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password123", repo.RegisteredUser.PasswordHash));
    }

    [Fact]
    public void Login_WhenCredentialsAreInvalid_ThrowsBadRequestException()
    {
        var repo = new FakeAuthRepository
        {
            UserByEmail = null
        };

        var service = new AuthService(repo, CreateConfig());

        Assert.Throws<BadRequestException>(() =>
            service.Login(new LoginDto
            {
                Email = "missing@example.com",
                Password = "Password123"
            }));
    }

    [Fact]
    public void Login_WhenCredentialsAreValid_ReturnsTokensAndUserDetails()
    {
        var repo = new FakeAuthRepository
        {
            UserByEmail = new User
            {
                UserID = 5,
                Username = "Ravi",
                Email = "ravi@example.com",
                Role = AppRoles.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            }
        };

        var service = new AuthService(repo, CreateConfig());

        var result = service.Login(new LoginDto
        {
            Email = "ravi@example.com",
            Password = "Password123"
        });

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal(5, result.UserId);
        Assert.Equal("Ravi", result.Name);
        Assert.Equal(AppRoles.Admin, result.Role);
        Assert.True(repo.CleanupExpiredRefreshTokensCalled);
        Assert.Equal(5, repo.RevokedActiveTokensUserId);
        Assert.Equal(5, repo.SavedRefreshTokenUserId);
        Assert.False(string.IsNullOrWhiteSpace(repo.SavedRefreshTokenHash));
        Assert.NotEqual(result.RefreshToken, repo.SavedRefreshTokenHash);
    }

    [Fact]
    public void UpdateEmail_WhenRepositoryReturnsFalse_ThrowsNotFoundException()
    {
        var repo = new FakeAuthRepository
        {
            UpdateEmailResult = false
        };

        var service = new AuthService(repo, CreateConfig());

        Assert.Throws<NotFoundException>(() =>
            service.UpdateEmail(10, "new@example.com"));
    }

    [Fact]
    public void ValidateRefreshToken_WhenTokenIsExpired_ReturnsNull()
    {
        var repo = new FakeAuthRepository
        {
            RefreshTokenToReturn = new RefreshToken
            {
                UserId = 1,
                Token = "hashed-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
            }
        };

        var service = new AuthService(repo, CreateConfig());

        var result = service.ValidateRefreshToken("plain-token");

        Assert.Null(result);
    }

    [Fact]
    public void RotateRefreshToken_WhenTokenIsInvalid_ThrowsBadRequestException()
    {
        var repo = new FakeAuthRepository
        {
            RefreshTokenToReturn = null
        };

        var service = new AuthService(repo, CreateConfig());

        Assert.Throws<BadRequestException>(() =>
            service.RotateRefreshToken("bad-token"));
    }

    [Fact]
    public void AssignRoleSafely_WhenLastAdminWouldBeRemoved_ThrowsBadRequestException()
    {
        var repo = new FakeAuthRepository
        {
            UserById = new User
            {
                UserID = 1,
                Username = "Admin",
                Email = "admin@example.com",
                Role = AppRoles.Admin
            },
            AdminCount = 1
        };

        var service = new AuthService(repo, CreateConfig());

        Assert.Throws<BadRequestException>(() =>
            service.AssignRoleSafely(1, AppRoles.User));
    }

    [Fact]
    public void AssignRoleSafely_WhenUserExists_UpdatesRole()
    {
        var repo = new FakeAuthRepository
        {
            UserById = new User
            {
                UserID = 2,
                Username = "User",
                Email = "user@example.com",
                Role = AppRoles.User
            },
            UpdateRoleResult = true
        };

        var service = new AuthService(repo, CreateConfig());

        service.AssignRoleSafely(2, AppRoles.Admin);

        Assert.Equal(2, repo.UpdatedRoleUserId);
        Assert.Equal(AppRoles.Admin, repo.UpdatedRole);
    }
}