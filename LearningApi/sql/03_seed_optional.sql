USE LearningDB;
GO

IF NOT EXISTS (SELECT 1
FROM dbo.Students)
BEGIN
    INSERT INTO dbo.Students
        (Name, Email)
    VALUES
        (N'Atharv Warkari', N'atharv@email.com'),
        (N'Radhika Dixit', N'radhika@email.com'),
        (N'Amey Datar', N'amey@email.com'),
        (N'Anushka Shalu', N'anushka@email.com'),
        (N'Srushti More', N'srushti@email.com'),
        (N'Kaushal Tapray', N'kaushal@email.com');
END;
GO

IF NOT EXISTS (SELECT 1
FROM dbo.Courses)
BEGIN
    INSERT INTO dbo.Courses
        (CourseName, Fee, DurationWeeks)
    VALUES
        (N'Python Fundamentals', 3999.00, 8),
        (N'SQL Fundamentals', 4999.00, 8),
        (N'ASP.NET Core Web API', 8999.00, 8),
        (N'Angular Fundamentals', 6999.00, 6),
        (N'SQL Server Essentials', 5999.00, 4),
        (N'C# Programming Basics', 8999.00, 5),
        (N'Full Stack Project Workshop', 15999.00, 10);
END;
GO

IF NOT EXISTS (SELECT 1
FROM dbo.Enrollments)
BEGIN
    INSERT INTO dbo.Enrollments
        (StudentID, CourseID, EnrollmentDate)
    VALUES
        (1, 1, SYSUTCDATETIME()),
        (1, 2, SYSUTCDATETIME()),
        (2, 3, SYSUTCDATETIME()),
        (3, 1, SYSUTCDATETIME()),
        (4, 4, SYSUTCDATETIME()),
        (5, 5, SYSUTCDATETIME()),
        (6, 3, SYSUTCDATETIME());

END;
GO
