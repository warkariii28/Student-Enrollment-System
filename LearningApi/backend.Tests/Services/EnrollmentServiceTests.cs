using LearningApi.Exceptions;
using LearningApi.Models;
using LearningApi.Services;
using backend.Tests.Fakes;

namespace backend.Tests.Services;

public class EnrollmentServiceTests
{
    [Fact]
    public void Add_SendsEnrollmentToRepository()
    {
        var repo = new FakeEnrollmentRepository();
        var service = new EnrollmentService(repo);

        service.Add(new Enrollment
        {
            StudentID = 1,
            CourseID = 2
        });

        Assert.NotNull(repo.AddedEnrollment);
        Assert.Equal(1, repo.AddedEnrollment.StudentID);
        Assert.Equal(2, repo.AddedEnrollment.CourseID);
    }

    [Fact]
    public void Delete_WhenIdIsInvalid_ThrowsBadRequestException()
    {
        var repo = new FakeEnrollmentRepository();
        var service = new EnrollmentService(repo);

        Assert.Throws<BadRequestException>(() => service.Delete(0));
    }

    [Fact]
    public void Delete_WhenRepositoryReturnsFalse_ThrowsNotFoundException()
    {
        var repo = new FakeEnrollmentRepository
        {
            DeleteResult = false
        };

        var service = new EnrollmentService(repo);

        Assert.Throws<NotFoundException>(() => service.Delete(99));
    }

    [Fact]
    public void Delete_WhenRepositoryReturnsTrue_DeletesEnrollment()
    {
        var repo = new FakeEnrollmentRepository
        {
            DeleteResult = true
        };

        var service = new EnrollmentService(repo);

        service.Delete(7);

        Assert.Equal(7, repo.DeletedId);
    }
}