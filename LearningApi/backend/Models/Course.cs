namespace LearningApi.Models;

public class Course
{
    public int CourseID { get; set; }
    public string CourseName { get; set; } = "";
    public decimal Fee { get; set; }
    public int DurationWeeks { get; set; }
}