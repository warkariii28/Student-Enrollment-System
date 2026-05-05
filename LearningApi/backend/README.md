# LearningApi Backend

This is the ASP.NET Core Web API backend for the LearningApi project. It exposes authenticated APIs for users, students, courses, and enrollments, backed by SQL Server stored procedures.

## Tech Stack

- ASP.NET Core Web API
- .NET 10
- SQL Server
- Microsoft.Data.SqlClient
- JWT bearer authentication
- BCrypt password hashing
- Swagger via Swashbuckle

## Architecture

The backend follows a simple layered structure:

```txt
Controller -> Service -> Repository -> SQL Server
```

Folders:

```txt
Controllers/       HTTP API endpoints
DTOs/              Request and response objects
Exceptions/        Custom app exceptions
Helpers/           API response helpers
Middleware/        Global exception middleware
Models/            Domain/database models
Repositories/      SQL Server data access
Services/          Business logic
```

## Prerequisites

- .NET SDK 10 or compatible runtime for this project
- SQL Server or SQL Server Express
- Database scripts from the root `sql/` folder

## Database Setup

Run these scripts from the repository root in order:

```txt
sql/00_create_database.sql
sql/01_schema.sql
sql/02_stored_procedures.sql
sql/03_seed_optional.sql
```

The API expects:

- Database name: `LearningDB`
- Connection string key: `DefaultConnection`

## Configuration

Create a local appsettings file:

```bash
copy appsettings.json.example appsettings.json
```

Update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LearningDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "replace-with-a-strong-secret-key-at-least-32-characters",
    "Issuer": "LearningApi",
    "Audience": "LearningApiUsers"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  }
}
```

Important: `Jwt:Key` must be at least 32 characters because the API validates this during startup.

## Run

From the repository root:

```bash
dotnet run --project backend/LearningApi.csproj
```

Or from the backend folder:

```bash
dotnet run
```

Development URLs:

```txt
http://localhost:5140
https://localhost:7175
```

Swagger:

```txt
http://localhost:5140/swagger
```

## Build

```bash
dotnet build backend/LearningApi.csproj
```

## Main API Endpoints

Auth:

```txt
POST api/auth/register
POST api/auth/login
POST api/auth/refresh
PUT  api/auth/update-email
PUT  api/auth/assign-role
```

Students:

```txt
GET    api/students
GET    api/students/{id}
POST   api/students              Admin only
PUT    api/students/{id}         Admin only
DELETE api/students/{id}         Admin only
GET    api/students/me
```

Courses:

```txt
GET    api/courses
GET    api/courses/{id}
POST   api/courses               Admin only
PUT    api/courses/{id}          Admin only
DELETE api/courses/{id}          Admin only
```

Enrollments:

```txt
GET    api/enrollments
POST   api/enrollments           Admin only
DELETE api/enrollments/{id}      Admin only
```

## Response Shape

Most API responses use this shape:

```json
{
  "success": true,
  "message": "Operation completed",
  "data": {}
}
```

Validation and handled errors return the same general response shape with `success: false`.

## Auth Flow

1. Register a user through `POST api/auth/register`.
2. Login through `POST api/auth/login`.
3. Store the returned JWT on the frontend.
4. Send the JWT in the `Authorization` header:

```txt
Authorization: Bearer YOUR_TOKEN
```

Admin-only operations require the user role to be `Admin`.

## Beginner Reading Order

Start here:

```txt
Program.cs
Middleware/GlobalExceptionMiddleware.cs
Controllers/AuthController.cs
Services/AuthService.cs
Repositories/AuthRepository.cs
Controllers/StudentsController.cs
Services/StudentService.cs
Repositories/StudentRepository.cs
```

This path shows how a request enters the API, gets validated, passes through service logic, reaches SQL Server, and returns a consistent response.
