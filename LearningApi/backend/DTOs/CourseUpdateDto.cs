using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class CourseUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string CourseName { get; set; } = "";

    [Required]
<<<<<<< HEAD
    [Range(typeof(decimal), "1", "1000000")]
=======
    [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "Fee must be greater than 0")]
>>>>>>> 3b0d457 (Add DTOs)
    public decimal Fee { get; set; }

    [Required]
    [Range(1, 52, ErrorMessage = "Duration must be between 1 and 52 weeks")]
    public int DurationWeeks { get; set; }
}