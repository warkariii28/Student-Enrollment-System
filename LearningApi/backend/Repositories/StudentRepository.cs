namespace LearningApi.Repositories;

using LearningApi.DTOs;
using LearningApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

public class StudentRepository : IStudentRepository
{
    private readonly string _conn;

    public StudentRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public List<StudentResponseDto> GetAll()
    {
        var students = new List<StudentResponseDto>();

        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetAllStudents", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            students.Add(new StudentResponseDto
            {
                StudentID = Convert.ToInt32(reader["StudentID"]),
                Name = reader["Name"]?.ToString() ?? "",
                Email = reader["Email"]?.ToString() ?? ""
            });
        }

        return students;
    }

    public Student? GetById(int id)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetStudentByID", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentID", id);

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return new Student
        {
            StudentID = (int)reader["StudentID"],
            Name = reader["Name"]?.ToString() ?? "",
            Email = reader["Email"]?.ToString() ?? ""
        };
    }

    public int Add(Student student)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("AddStudent", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Name", student.Name);
        cmd.Parameters.AddWithValue("@Email", student.Email);

        conn.Open();

        var result = cmd.ExecuteScalar();

        return Convert.ToInt32(result);
    }

    public bool Delete(int id)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RemoveStudent", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentID", id);

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public bool Update(Student student)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("UpdateStudent", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
        cmd.Parameters.AddWithValue("@Name", student.Name);
        cmd.Parameters.AddWithValue("@Email", student.Email);

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;

    }
}