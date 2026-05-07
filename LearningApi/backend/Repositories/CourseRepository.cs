using LearningApi.DTOs;
using LearningApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LearningApi.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly string _conn;

    public CourseRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public List<CourseResponseDto> GetAll()
    {
        var courses = new List<CourseResponseDto>();

        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetAllCourses", conn);
        cmd.CommandTimeout = 30;
        cmd.CommandType = CommandType.StoredProcedure;

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            courses.Add(new CourseResponseDto
            {
                CourseID = Convert.ToInt32(reader["CourseID"]),
                CourseName = reader["CourseName"]?.ToString() ?? "",
                Fee = Convert.ToDecimal(reader["Fee"]),
                DurationWeeks = Convert.ToInt32(reader["DurationWeeks"])
            });
        }

        return courses;
    }

    public int Add(Course course)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("AddCourse", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@CourseName", SqlDbType.NVarChar, 200).Value = course.CourseName;
        cmd.Parameters.Add("@Fee", SqlDbType.Decimal).Value = course.Fee;
        cmd.Parameters.Add("@DurationWeeks", SqlDbType.Int).Value = course.DurationWeeks;

        conn.Open();

        var result = cmd.ExecuteScalar();

        return Convert.ToInt32(result);
    }

    public bool Delete(int id)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("RemoveCourse", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@CourseID", SqlDbType.Int).Value = id;

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }

    public Course? GetById(int id)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetCourseByID", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@CourseID", SqlDbType.Int).Value = id;

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Course
            {
                CourseID = (int)reader["CourseID"],
                CourseName = reader["CourseName"].ToString() ?? "",
                Fee = (decimal)reader["Fee"],
                DurationWeeks = (int)reader["DurationWeeks"]
            };
        }

        return null;
    }

    public bool Update(Course course)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("UpdateCourse", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@CourseID", SqlDbType.Int).Value = course.CourseID;
        cmd.Parameters.Add("@CourseName", SqlDbType.NVarChar, 200).Value = course.CourseName;
        cmd.Parameters.Add("@Fee", SqlDbType.Decimal).Value = course.Fee;
        cmd.Parameters.Add("@DurationWeeks", SqlDbType.Int).Value = course.DurationWeeks;

        conn.Open();

        int rows = cmd.ExecuteNonQuery();

        return rows > 0;
    }
}