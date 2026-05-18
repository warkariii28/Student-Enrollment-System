USE LearningDB;
GO

-- Checks to run after setup:
/* SELECT * FROM dbo.Students;
SELECT * FROM dbo.Users;
SELECT * FROM dbo.Enrollments;
SELECT * FROM dbo.Courses;
SELECT * FROM dbo.RefreshTokens;
SELECT * FROM dbo.AdminAuditLogs; */


-- Optional admin promotion:
/* UPDATE dbo.Users
SET Role = N'Admin'
WHERE Email = N'ashw@email.com'; */

/* 
SELECT *
FROM dbo.RefreshTokens
WHERE ExpiresAt < SYSUTCDATETIME(); */

SELECT UserId, IsRevoked, ExpiresAt
FROM dbo.RefreshTokens
WHERE UserId = 1
ORDER BY ExpiresAt DESC;