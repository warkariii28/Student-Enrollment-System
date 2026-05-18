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

    public PagedResultDto<EnrollmentResponseDto> GetPaged(int page, int pageSize)
    {
        var enrollments = new List<EnrollmentResponseDto>();
        var totalCount = 0;

        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetEnrollmentsPaged", conn);

        cmd.CommandTimeout = 30;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@Page", SqlDbType.Int).Value = page;
        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            enrollments.Add(new EnrollmentResponseDto
            {
                EnrollmentID = Convert.ToInt32(reader["EnrollmentID"]),
                StudentName = reader["StudentName"]?.ToString() ?? "",
                CourseName = reader["CourseName"]?.ToString() ?? "",
                EnrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"])
            });
        }

        if (reader.NextResult() && reader.Read())
        {
            totalCount = Convert.ToInt32(reader["TotalCount"]);
        }

        return new PagedResultDto<EnrollmentResponseDto>
        {
            Items = enrollments,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
    
    public void Add(Enrollment enrollment)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("EnrollStudent", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = enrollment.StudentID;
        cmd.Parameters.Add("@CourseID", SqlDbType.Int).Value = enrollment.CourseID;

        conn.Open();

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqlException)
        {
            throw new BadRequestException("Invalid enrollment request");
        }
    }

    public bool Delete(int id)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RemoveEnrollment", conn);
        cmd.CommandTimeout = 30;
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@EnrollmentID", SqlDbType.Int).Value = id;

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }
}