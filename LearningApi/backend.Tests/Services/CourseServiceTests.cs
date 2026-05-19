using LearningApi.Exceptions;
using LearningApi.Models;
using LearningApi.Services;
using backend.Tests.Fakes;

namespace backend.Tests.Services;

public class CourseServiceTests
{
    [Fact]
    public void GetById_WhenCourseExists_ReturnsCourse()
    {
        var repo = new FakeCourseRepository
        {
            CourseToReturn = new Course
            {
                CourseID = 1,
                CourseName = "ASP.NET Core",
                Fee = 4999,
                DurationWeeks = 8
            }
        };

        var service = new CourseService(repo);

        var result = service.GetById(1);

        Assert.Equal(1, result.CourseID);
        Assert.Equal("ASP.NET Core", result.CourseName);
    }

    [Fact]
    public void GetById_WhenCourseMissing_ThrowsNotFoundException()
    {
        var repo = new FakeCourseRepository
        {
            CourseToReturn = null
        };

        var service = new CourseService(repo);

        Assert.Throws<NotFoundException>(() => service.GetById(99));
    }

    [Fact]
    public void Delete_WhenIdIsInvalid_ThrowsBadRequestException()
    {
        var repo = new FakeCourseRepository();
        var service = new CourseService(repo);

        Assert.Throws<BadRequestException>(() => service.Delete(0));
    }

    [Fact]
    public void Delete_WhenRepositoryReturnsFalse_ThrowsNotFoundException()
    {
        var repo = new FakeCourseRepository
        {
            DeleteResult = false
        };

        var service = new CourseService(repo);

        Assert.Throws<NotFoundException>(() => service.Delete(99));
    }

    [Fact]
    public void Delete_WhenRepositoryReturnsTrue_DeletesCourse()
    {
        var repo = new FakeCourseRepository
        {
            DeleteResult = true
        };

        var service = new CourseService(repo);

        service.Delete(5);

        Assert.Equal(5, repo.DeletedId);
    }

    [Fact]
    public void Update_WhenRepositoryReturnsFalse_ThrowsNotFoundException()
    {
        var repo = new FakeCourseRepository
        {
            UpdateResult = false
        };

        var service = new CourseService(repo);

        Assert.Throws<NotFoundException>(() =>
            service.Update(new Course
            {
                CourseID = 99,
                CourseName = "Missing",
                Fee = 1000,
                DurationWeeks = 4
            }));
    }

    [Fact]
    public void Add_ReturnsNewCourseId()
    {
        var repo = new FakeCourseRepository();
        var service = new CourseService(repo);

        var id = service.Add(new Course
        {
            CourseName = "Angular",
            Fee = 3999,
            DurationWeeks = 6
        });

        Assert.Equal(20, id);
        Assert.NotNull(repo.AddedCourse);
        Assert.Equal("Angular", repo.AddedCourse.CourseName);
    }
}