using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class CourseCreateDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    [RegularExpression(@"^(?=.*[A-Za-z])[A-Za-z0-9\s.'+#&()-]+$", ErrorMessage = "Course name must include letters and valid course characters only.")]
    public string CourseName { get; set; } = "";

    [Required]
    [Range(typeof(decimal), "1", "1000000")]
    public decimal Fee { get; set; }

    [Required]
    [Range(1, 52, ErrorMessage = "Duration must be between 1 and 52 weeks")]
    public int DurationWeeks { get; set; }
}