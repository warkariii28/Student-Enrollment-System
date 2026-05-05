USE LearningDB;
GO

IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        StudentID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Students PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Students_Email' AND object_id = OBJECT_ID(N'dbo.Students'))
BEGIN
    CREATE UNIQUE INDEX UX_Students_Email ON dbo.Students(Email);
END;
GO

IF OBJECT_ID(N'dbo.Courses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Courses
    (
        CourseID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Courses PRIMARY KEY,
        CourseName NVARCHAR(200) NOT NULL,
        Fee DECIMAL(18,2) NOT NULL,
        DurationWeeks INT NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Courses_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_Courses_Fee CHECK (Fee BETWEEN 1 AND 1000000),
        CONSTRAINT CK_Courses_DurationWeeks CHECK (DurationWeeks BETWEEN 1 AND 52)
    );
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Role DEFAULT N'User',
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_Users_Role CHECK (Role IN (N'User', N'Admin'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Users_Email' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users(Email);
END;
GO

IF OBJECT_ID(N'dbo.Enrollments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Enrollments
    (
        EnrollmentID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Enrollments PRIMARY KEY,
        StudentID INT NOT NULL,
        CourseID INT NOT NULL,
        EnrollmentDate DATETIME2(0) NOT NULL CONSTRAINT DF_Enrollments_EnrollmentDate DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentID) REFERENCES dbo.Students(StudentID) ON DELETE CASCADE,
        CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseID) REFERENCES dbo.Courses(CourseID) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Enrollments_StudentID_CourseID' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
    CREATE UNIQUE INDEX UX_Enrollments_StudentID_CourseID ON dbo.Enrollments(StudentID, CourseID);
END;
GO

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        RefreshTokenID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RefreshTokens PRIMARY KEY,
        UserId INT NOT NULL,
        Token NVARCHAR(512) NOT NULL,
        ExpiresAt DATETIME2(0) NOT NULL,
        IsRevoked BIT NOT NULL CONSTRAINT DF_RefreshTokens_IsRevoked DEFAULT 0,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserID) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RefreshTokens_Token' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
BEGIN
    CREATE UNIQUE INDEX UX_RefreshTokens_Token ON dbo.RefreshTokens(Token);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
BEGIN
    CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens(UserId);
END;
GO
