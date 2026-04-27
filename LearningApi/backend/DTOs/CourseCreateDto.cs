using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class CourseCreateDto
{
    [Required]
    public string CourseName { get; set; } = "";

    [Required]
    public decimal Fee { get; set; }

    [Required]
    public int DurationWeeks { get; set; }
}