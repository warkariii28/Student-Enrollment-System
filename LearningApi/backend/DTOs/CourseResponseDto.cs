namespace LearningApi.DTOs;

public class CourseResponseDto
{
    public int CourseID { get; set; }
    public string CourseName { get; set; } = "";
    public decimal Fee { get; set; }
    public int DurationWeeks { get; set; }
}