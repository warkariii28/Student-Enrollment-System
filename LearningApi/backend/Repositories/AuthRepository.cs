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
            PasswordHash = reader["PasswordHash"].ToString() ?? ""
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
}