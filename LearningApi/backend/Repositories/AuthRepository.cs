using LearningApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using LearningApi.Exceptions;

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
        cmd.CommandTimeout = 30;
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = user.Email;
        cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = user.PasswordHash;
        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = user.Role;

        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
        {
            throw new BadRequestException("Email already exists");
        }
    }

    public User? GetByEmail(string email)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetUserByEmail", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = email;


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

        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = email;


        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public bool UpdateRole(int userId, string role)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("UpdateUserRole", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = role;


        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public void SaveRefreshToken(int userId, string token)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "INSERT INTO RefreshTokens (UserId, TokenHash, ExpiresAt) VALUES (@u, @t, @e)", conn);

        cmd.Parameters.Add("@u", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@t", SqlDbType.NVarChar, 88).Value = token;
        cmd.Parameters.Add("@e", SqlDbType.DateTime).Value = DateTime.UtcNow.AddDays(7);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void RevokeRefreshToken(string token)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand(
            "UPDATE RefreshTokens SET IsRevoked = 1 WHERE TokenHash = @t", conn);

        cmd.Parameters.Add("@t", SqlDbType.NVarChar, 88).Value = token;


        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public RefreshToken? GetRefreshToken(string token)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            "SELECT * FROM RefreshTokens WHERE TokenHash=@t AND IsRevoked=0", conn);

        cmd.Parameters.Add("@t", SqlDbType.NVarChar, 88).Value = token;
        conn.Open();

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new RefreshToken
        {
            UserId = (int)r["UserId"],
            Token = r["TokenHash"].ToString()!,
            ExpiresAt = (DateTime)r["ExpiresAt"]
        };
    }

    public User? GetById(int userId)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand(
            "SELECT UserID, Username, Email, PasswordHash, Role FROM Users WHERE UserID = @id", conn);

        cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;

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
