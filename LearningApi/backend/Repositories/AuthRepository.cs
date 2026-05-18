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
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("SaveRefreshToken", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
        cmd.Parameters.Add("@TokenHash", SqlDbType.NVarChar, 88).Value = token;
        cmd.Parameters.Add("@ExpiresAt", SqlDbType.DateTime).Value = DateTime.UtcNow.AddDays(7);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void RevokeRefreshToken(string token)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RevokeRefreshToken", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@TokenHash", SqlDbType.NVarChar, 88).Value = token;

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public RefreshToken? GetRefreshToken(string token)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetRefreshToken", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@TokenHash", SqlDbType.NVarChar, 88).Value = token;

        conn.Open();

        using SqlDataReader r = cmd.ExecuteReader();

        if (!r.Read())
            return null;

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
        using SqlCommand cmd = new SqlCommand("GetUserByID", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

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
    public void CleanupExpiredRefreshTokens()
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("CleanupExpiredRefreshTokens", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        conn.Open();
        cmd.ExecuteNonQuery();
    }
    public void RevokeActiveRefreshTokensForUser(int userId)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RevokeActiveRefreshTokensForUser", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

        conn.Open();
        cmd.ExecuteNonQuery();
    }
}
