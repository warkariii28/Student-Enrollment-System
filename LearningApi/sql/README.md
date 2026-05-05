# LearningDB SQL Reconstruction

These scripts were reconstructed from the current API repositories, DTOs, frontend models, and `docs/PROJECT_GRAPH.md`.

Run order:

1. `00_create_database.sql`
2. `01_schema.sql`
3. `02_stored_procedures.sql`
4. `03_seed_optional.sql` if you want starter course data

The backend expects SQL Server, database name `LearningDB`, and the connection string key `DefaultConnection`.

## Reconstructed Contract

Tables:

- `Students`: `StudentID`, `Name`, `Email`
- `Courses`: `CourseID`, `CourseName`, `Fee`, `DurationWeeks`
- `Enrollments`: `EnrollmentID`, `StudentID`, `CourseID`, `EnrollmentDate`
- `Users`: `UserID`, `Username`, `Email`, `PasswordHash`, `Role`
- `RefreshTokens`: `UserId`, `Token`, `ExpiresAt`, `IsRevoked`

Stored procedures used by the API:

- Students: `GetAllStudents`, `GetStudentByID`, `AddStudent`, `UpdateStudent`, `RemoveStudent`
- Courses: `GetAllCourses`, `GetCourseByID`, `AddCourse`, `UpdateCourse`, `RemoveCourse`
- Enrollments: `GetEnrollmentDetails`, `EnrollStudent`, `RemoveEnrollment`
- Auth: `RegisterUsers`, `GetUserByEmail`, `UpdateUserEmail`, `UpdateUserRole`

`AuthRepository` also uses direct SQL for refresh token insert/revoke/read and `GetById`.
