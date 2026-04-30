using LearningApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LearningApi.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly string _conn;

    public AuthRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public void Register(User user)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RegisterUsers", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Username", user.Username);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@Role", user.Role);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public User? GetByEmail(string email)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetUserByEmail", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Email", email);

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return new User
        {
            UserID = (int)reader["UserID"],
            Username = reader["Username"].ToString() ?? "",
            Email = reader["Email"].ToString() ?? "",
            PasswordHash = reader["PasswordHash"].ToString() ?? "",
            Role = reader["Role"].ToString() ?? ""
        };
    }

    public bool UpdateEmail(int userId, string email)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("UpdateUserEmail", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@UserID", userId);
        cmd.Parameters.AddWithValue("@Email", email);

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public bool UpdateRole(int userId, string role)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("UpdateUserRole", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@UserID", userId);
        cmd.Parameters.AddWithValue("@Role", role);

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public void SaveRefreshToken(int userId, string token)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "INSERT INTO RefreshTokens (UserId, Token, ExpiresAt) VALUES (@u, @t, @e)", conn);

        cmd.Parameters.AddWithValue("@u", userId);
        cmd.Parameters.AddWithValue("@t", token);
        cmd.Parameters.AddWithValue("@e", DateTime.Now.AddDays(7));

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void RevokeRefreshToken(string token)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand(
            "UPDATE RefreshTokens SET IsRevoked = 1 WHERE Token = @t", conn);

        cmd.Parameters.AddWithValue("@t", token);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public RefreshToken? GetRefreshToken(string token)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "SELECT * FROM RefreshTokens WHERE Token=@t AND IsRevoked=0", conn);

        cmd.Parameters.AddWithValue("@t", token);
        conn.Open();

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new RefreshToken
        {
            UserId = (int)r["UserId"],
            Token = r["Token"].ToString()!,
            ExpiresAt = (DateTime)r["ExpiresAt"]
        };
    }

    public User? GetById(int userId)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand(
            "SELECT UserID, Username, Email, PasswordHash, Role FROM Users WHERE UserID = @id", conn);

        cmd.Parameters.AddWithValue("@id", userId);

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        if (!reader.Read()) return null;

        return new User
        {
            UserID = (int)reader["UserID"],
            Username = reader["Username"].ToString() ?? "",
            Email = reader["Email"].ToString() ?? "",
            PasswordHash = reader["PasswordHash"].ToString() ?? "",
            Role = reader["Role"].ToString() ?? ""
        };
    }
}