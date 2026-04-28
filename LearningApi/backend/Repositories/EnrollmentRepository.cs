using Microsoft.Data.SqlClient;
using System.Data;
using LearningApi.Models;
using LearningApi.DTOs;
using LearningApi.Exceptions;

namespace LearningApi.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly string _conn;

    public EnrollmentRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public List<EnrollmentResponseDto> GetAll()
    {
        var list = new List<EnrollmentResponseDto>();

        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetEnrollmentDetails", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new EnrollmentResponseDto
            {
                EnrollmentID = Convert.ToInt32(reader["EnrollmentID"]),
                StudentName = reader["StudentName"].ToString() ?? "",
                CourseName = reader["CourseName"].ToString() ?? "",
                EnrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"])
            });
        }

        return list;
    }

    public void Add(Enrollment enrollment)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("EnrollStudent", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@StudentID", enrollment.StudentID);
        cmd.Parameters.AddWithValue("@CourseID", enrollment.CourseID);

        conn.Open();

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public bool Delete(int id)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RemoveEnrollment", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@EnrollmentID", id);

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }
}