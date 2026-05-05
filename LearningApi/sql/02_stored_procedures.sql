USE LearningDB;
GO

CREATE OR ALTER PROCEDURE dbo.GetAllStudents
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentID, Name, Email
    FROM dbo.Students
    ORDER BY StudentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetStudentByID
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT StudentID, Name, Email
    FROM dbo.Students
    WHERE StudentID = @StudentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.AddStudent
    @Name NVARCHAR(100),
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Students (Name, Email)
    VALUES (@Name, @Email);

    SELECT CONVERT(INT, SCOPE_IDENTITY()) AS StudentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateStudent
    @StudentID INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE dbo.Students
    SET Name = @Name,
        Email = @Email
    WHERE StudentID = @StudentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.RemoveStudent
    @StudentID INT
AS
BEGIN
    SET NOCOUNT OFF;

    DELETE FROM dbo.Students
    WHERE StudentID = @StudentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetAllCourses
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CourseID, CourseName, Fee, DurationWeeks
    FROM dbo.Courses
    ORDER BY CourseID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetCourseByID
    @CourseID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CourseID, CourseName, Fee, DurationWeeks
    FROM dbo.Courses
    WHERE CourseID = @CourseID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.AddCourse
    @CourseName NVARCHAR(200),
    @Fee DECIMAL(18,2),
    @DurationWeeks INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Courses (CourseName, Fee, DurationWeeks)
    VALUES (@CourseName, @Fee, @DurationWeeks);

    SELECT CONVERT(INT, SCOPE_IDENTITY()) AS CourseID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateCourse
    @CourseID INT,
    @CourseName NVARCHAR(200),
    @Fee DECIMAL(18,2),
    @DurationWeeks INT
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE dbo.Courses
    SET CourseName = @CourseName,
        Fee = @Fee,
        DurationWeeks = @DurationWeeks
    WHERE CourseID = @CourseID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.RemoveCourse
    @CourseID INT
AS
BEGIN
    SET NOCOUNT OFF;

    DELETE FROM dbo.Courses
    WHERE CourseID = @CourseID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetEnrollmentDetails
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.EnrollmentID,
        s.Name AS StudentName,
        c.CourseName,
        e.EnrollmentDate
    FROM dbo.Enrollments e
    INNER JOIN dbo.Students s ON s.StudentID = e.StudentID
    INNER JOIN dbo.Courses c ON c.CourseID = e.CourseID
    ORDER BY e.EnrollmentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.EnrollStudent
    @StudentID INT,
    @CourseID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE StudentID = @StudentID)
        THROW 50001, 'Student not found', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Courses WHERE CourseID = @CourseID)
        THROW 50002, 'Course not found', 1;

    IF EXISTS (SELECT 1 FROM dbo.Enrollments WHERE StudentID = @StudentID AND CourseID = @CourseID)
        THROW 50003, 'Student is already enrolled in this course', 1;

    INSERT INTO dbo.Enrollments (StudentID, CourseID)
    VALUES (@StudentID, @CourseID);
END;
GO

CREATE OR ALTER PROCEDURE dbo.RemoveEnrollment
    @EnrollmentID INT
AS
BEGIN
    SET NOCOUNT OFF;

    DELETE FROM dbo.Enrollments
    WHERE EnrollmentID = @EnrollmentID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.RegisterUsers
    @Username NVARCHAR(100),
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(255),
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users (Username, Email, PasswordHash, Role)
    VALUES (@Username, @Email, @PasswordHash, @Role);
END;
GO

CREATE OR ALTER PROCEDURE dbo.GetUserByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserID, Username, Email, PasswordHash, Role
    FROM dbo.Users
    WHERE Email = @Email;
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateUserEmail
    @UserID INT,
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE dbo.Users
    SET Email = @Email
    WHERE UserID = @UserID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.UpdateUserRole
    @UserID INT,
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE dbo.Users
    SET Role = @Role
    WHERE UserID = @UserID;
END;
GO
