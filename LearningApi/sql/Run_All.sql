--SELECT * from Students;
SELECT * from Users;
--SELECT * from Enrollments;
--SELECT * from Courses;

UPDATE dbo.Users
SET Role = 'Admin'
WHERE Email = 'aw@email.com';