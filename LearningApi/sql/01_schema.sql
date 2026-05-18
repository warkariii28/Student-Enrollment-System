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

ALTER TABLE dbo.Students
ADD CONSTRAINT UQ_Students_Email UNIQUE (Email);
GO

/* IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Students_Email' AND object_id = OBJECT_ID(N'dbo.Students'))
BEGIN
    CREATE UNIQUE INDEX UX_Students_Email ON dbo.Students(Email);
END;
GO */

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

ALTER TABLE dbo.Courses
ADD CONSTRAINT UQ_Courses_CourseName UNIQUE (CourseName);
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Role NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_Role DEFAULT N'User',
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT CK_Users_Role CHECK (Role IN (N'User', N'Admin'))
    );
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.Users', N'PasswordHash') < 1000
BEGIN
    ALTER TABLE dbo.Users ALTER COLUMN PasswordHash NVARCHAR(500) NOT NULL;
END;
GO

/* IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Users_Email' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE UNIQUE INDEX UX_Users_Email ON dbo.Users(Email);
END;
GO */

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

/* IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Enrollments_StudentID_CourseID' AND object_id = OBJECT_ID(N'dbo.Enrollments'))
BEGIN
    CREATE UNIQUE INDEX UX_Enrollments_StudentID_CourseID ON dbo.Enrollments(StudentID, CourseID);
END;
GO */

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        RefreshTokenID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RefreshTokens PRIMARY KEY,
        UserId INT NOT NULL,
        TokenHash NVARCHAR(88) NOT NULL,
        ExpiresAt DATETIME2(0) NOT NULL,
        IsRevoked BIT NOT NULL CONSTRAINT DF_RefreshTokens_IsRevoked DEFAULT 0,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserID) ON DELETE CASCADE
    );
END;
GO

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RefreshTokens', N'TokenHash') IS NULL
   AND COL_LENGTH(N'dbo.RefreshTokens', N'Token') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RefreshTokens_Token' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
    BEGIN
        DROP INDEX UX_RefreshTokens_Token ON dbo.RefreshTokens;
    END;

    EXEC sp_rename N'dbo.RefreshTokens.Token', N'TokenHash', N'COLUMN';
END;
GO

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.RefreshTokens', N'TokenHash') IS NOT NULL
   AND (
        SELECT max_length
        FROM sys.columns
        WHERE object_id = OBJECT_ID(N'dbo.RefreshTokens')
          AND name = N'TokenHash'
   ) <> 176
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RefreshTokens_TokenHash' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
    BEGIN
        DROP INDEX UX_RefreshTokens_TokenHash ON dbo.RefreshTokens;
    END;

    ALTER TABLE dbo.RefreshTokens ALTER COLUMN TokenHash NVARCHAR(88) NOT NULL;
END;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RefreshTokens_Token' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
BEGIN
    DROP INDEX UX_RefreshTokens_Token ON dbo.RefreshTokens;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RefreshTokens_TokenHash' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
BEGIN
    CREATE UNIQUE INDEX UX_RefreshTokens_TokenHash ON dbo.RefreshTokens(TokenHash);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
BEGIN
    CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens(UserId);
END;
GO

IF OBJECT_ID(N'dbo.AdminAuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AdminAuditLogs
    (
        AuditLogID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AdminAuditLogs PRIMARY KEY,
        AdminUserID INT NOT NULL,
        Action NVARCHAR(100) NOT NULL,
        EntityName NVARCHAR(100) NOT NULL,
        EntityID INT NULL,
        Details NVARCHAR(500) NULL,
        IpAddress NVARCHAR(100) NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_AdminAuditLogs_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_AdminAuditLogs_Users FOREIGN KEY (AdminUserID) REFERENCES dbo.Users(UserID)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AdminAuditLogs_AdminUserID' AND object_id = OBJECT_ID(N'dbo.AdminAuditLogs'))
BEGIN
    CREATE INDEX IX_AdminAuditLogs_AdminUserID ON dbo.AdminAuditLogs(AdminUserID);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AdminAuditLogs_CreatedAt' AND object_id = OBJECT_ID(N'dbo.AdminAuditLogs'))
BEGIN
    CREATE INDEX IX_AdminAuditLogs_CreatedAt ON dbo.AdminAuditLogs(CreatedAt);
END;
GO
