using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class CourseCreateDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string CourseName { get; set; } = "";

    [Required]
    [Range(0.01, decimal.MaxValue, ErrorMessage = "Fee must be greater than 0")]
    public decimal Fee { get; set; }

    [Required]
    [Range(1, 52, ErrorMessage = "Duration must be between 1 and 52 weeks")]
    public int DurationWeeks { get; set; }
}