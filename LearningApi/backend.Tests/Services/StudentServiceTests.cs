using BrightPath.Exceptions;
using BrightPath.Models;
using BrightPath.Services;
using backend.Tests.Fakes;

namespace backend.Tests.Services;

public class StudentServiceTests
{
    [Fact]
    public void GetById_WhenStudentExists_ReturnsStudent()
    {
        var repo = new FakeStudentRepository
        {
            StudentToReturn = new Student
            {
                StudentID = 1,
                Name = "Asha",
                Email = "asha@example.com"
            }
        };

        var service = new StudentService(repo);

        var result = service.GetById(1);

        Assert.Equal(1, result.StudentID);
        Assert.Equal("Asha", result.Name);
    }

    [Fact]
    public void GetById_WhenStudentMissing_ThrowsNotFoundException()
    {
        var repo = new FakeStudentRepository
        {
            StudentToReturn = null
        };

        var service = new StudentService(repo);

        Assert.Throws<NotFoundException>(() => service.GetById(99));
    }

    [Fact]
    public void Delete_WhenIdIsInvalid_ThrowsBadRequestException()
    {
        var repo = new FakeStudentRepository();
        var service = new StudentService(repo);

        Assert.Throws<BadRequestException>(() => service.Delete(0));
    }

    [Fact]
    public void Delete_WhenRepositoryReturnsFalse_ThrowsNotFoundException()
    {
        var repo = new FakeStudentRepository
        {
            DeleteResult = false
        };

        var service = new StudentService(repo);

        Assert.Throws<NotFoundException>(() => service.Delete(99));
    }

    [Fact]
    public void Delete_WhenRepositoryReturnsTrue_DeletesStudent()
    {
        var repo = new FakeStudentRepository
        {
            DeleteResult = true
        };

        var service = new StudentService(repo);

        service.Delete(5);

        Assert.Equal(5, repo.DeletedId);
    }

    [Fact]
    public void Update_WhenRepositoryReturnsFalse_ThrowsNotFoundException()
    {
        var repo = new FakeStudentRepository
        {
            UpdateResult = false
        };

        var service = new StudentService(repo);

        Assert.Throws<NotFoundException>(() =>
            service.Update(new Student
            {
                StudentID = 99,
                Name = "Missing",
                Email = "missing@example.com"
            }));
    }

    [Fact]
    public void Add_ReturnsNewStudentId()
    {
        var repo = new FakeStudentRepository();
        var service = new StudentService(repo);

        var id = service.Add(new Student
        {
            Name = "Ravi",
            Email = "ravi@example.com"
        });

        Assert.Equal(10, id);
        Assert.NotNull(repo.AddedStudent);
        Assert.Equal("Ravi", repo.AddedStudent.Name);
    }
}