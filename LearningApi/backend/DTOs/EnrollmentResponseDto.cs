namespace LearningApi.DTOs;

public class EnrollmentResponseDto
{
    public int EnrollmentID { get; set; }
    public string StudentName { get; set; } = "";
    public string CourseName { get; set; } = "";
    public DateTime EnrollmentDate { get; set; }
}