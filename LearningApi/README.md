# BrightPath

BrightPath is a beginner-friendly full-stack student learning management project. It combines an Angular frontend, an ASP.NET Core Web API backend, and SQL Server stored procedures to demonstrate a realistic Controller -> Service -> Repository -> Database workflow.

## What The Project Covers

- JWT login, register, refresh token storage, and role-based access
- Protected Angular routes with guards and interceptors
- Student, course, and enrollment dashboards
- Search, pagination, loading states, toast notifications, and confirm dialogs
- ASP.NET Core controllers, DTO validation, services, repositories, and middleware
- SQL Server schema, seed data, and stored procedures

## Tech Stack

- Frontend: Angular 21, TypeScript, RxJS, Angular signals, lucide-angular
- Backend: ASP.NET Core Web API on .NET 10
- Database: SQL Server
- Auth: JWT bearer authentication and role-based authorization
- Styling: CSS custom properties in `frontend/src/styles/tokens.css`

## Project Structure

```txt
BrightPath/
  backend/          ASP.NET Core Web API
  frontend/         Angular application
  sql/              SQL Server database scripts
  docs/             Project notes and graph documentation
```

## Prerequisites

- .NET SDK 10 or compatible preview/runtime for this project
- Node.js and npm
- SQL Server or SQL Server Express
- Angular CLI, optional but useful

## Database Setup

Run the SQL scripts in this order:

```txt
sql/00_create_database.sql
sql/01_schema.sql
sql/02_stored_procedures.sql
sql/03_seed_optional.sql
```

The backend expects a SQL Server database named `LearningDB` and a connection string named `DefaultConnection`.

## Backend Setup

From the repository root:

```bash
cd backend
copy appsettings.json.example appsettings.json
dotnet restore
dotnet run
```

Update `backend/appsettings.json` before running:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LearningDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "replace-with-a-strong-secret-key-at-least-32-characters",
    "Issuer": "BrightPath",
    "Audience": "BrightPathUsers"
  }
}
```

The API runs at:

```txt
http://localhost:5140
https://localhost:7175
```

Swagger is available in development at:

```txt
http://localhost:5140/swagger
```

## Production Configuration

Use environment-specific configuration for production. Do not commit real secrets in `appsettings.json`.

Recommended environment variables:

```bash
ConnectionStrings__DefaultConnection="Server=YOUR_SERVER;Database=LearningDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=False;"
Jwt__Key="replace-with-a-strong-random-secret-at-least-32-characters"
Jwt__Issuer="BrightPath"
Jwt__Audience="BrightPathUsers"
Cors__AllowedOrigins__0="https://your-frontend-domain.com"
ASPNETCORE_ENVIRONMENT="Production"
```

Production checklist:

- Set `Jwt:Key` to a strong secret with at least 32 characters.
- Restrict `Cors:AllowedOrigins` to the real frontend domain.
- Use HTTPS for the API and frontend.
- Use a production SQL Server connection string instead of trusted local development auth.
- Keep Swagger enabled only for environments where API exploration is intended.

## Frontend Setup

From the repository root:

```bash
cd frontend
npm install
npm start
```

The Angular app runs at:

```txt
http://localhost:4200
```

The frontend API base URL is configured in:

```txt
frontend/src/environments/environment.ts
```

Default value:

```ts
apiBaseUrl: 'http://localhost:5140'
```

## Main App Routes

- `/login` - sign in
- `/register` - create account
- `/dashboard` - students dashboard
- `/dashboard/add` - add student, admin only
- `/dashboard/edit/:id` - edit student, admin only
- `/dashboard/courses` - courses dashboard
- `/dashboard/courses/add` - add course, admin only
- `/dashboard/courses/edit/:id` - edit course, admin only
- `/dashboard/enrollments` - enrollments dashboard
- `/dashboard/enrollments/add` - add enrollment, admin only

## Useful Commands

Backend:

```bash
dotnet build backend/BrightPath.csproj
dotnet run --project backend/BrightPath.csproj
```

Frontend:

```bash
cd frontend
npm run build
npm test
```

## API Areas

- `api/auth` - register, login, refresh token, email update, role assignment
- `api/students` - student records
- `api/courses` - course catalog
- `api/enrollments` - student-course registrations

Most write operations require an authenticated admin user.

## Learning Notes

This project is intentionally structured for learning. Good next exercises are:

- Add a README screenshot section
- Add backend unit tests for services
- Add frontend component tests for table/search behavior
- Split development-only controllers away from production API routes
- Clean CSS budget warnings in the navbar and sidebar
