# LearningApi

Full-stack learning management project with an ASP.NET Core Web API backend and an Angular frontend.

## Project Overview

This repository includes:
- `LearningApi/backend/`: ASP.NET Core Web API targeting `net10.0`
- `LearningApi/frontend/`: Angular 21 single-page application
- `LearningApi/docs/PROJECT_GRAPH.md`: generated architecture and project map

The system implements:
- JWT authentication for secure API access
- SQL Server data persistence via repositories and stored procedures
- Angular client with login, registration, course, student, and enrollment pages

---

## Architecture

### Backend

The backend is an ASP.NET Core Web API with the following structure:
- `Controllers/` - HTTP controllers for auth, students, courses, enrollments, and test helpers
- `Services/` - business logic layer with interfaces and implementations
- `Repositories/` - data access layer using SQL Server
- `Models/` - domain models and request/response DTOs
- `Middleware/` - global exception handling

Key backend features:
- JWT authentication with `Microsoft.AspNetCore.Authentication.JwtBearer`
- Password hashing with `BCrypt.Net-Next`
- Swagger / OpenAPI support for API testing
- CORS policy configured for `http://localhost:4200`

### Frontend

The frontend is an Angular 21 application built with:
- `@angular/core`, `@angular/router`, `@angular/forms`
- `rxjs`
- `vitest` for unit testing

Frontend structure includes:
- `src/app/core/` - shared models, services, interceptors, and utilities
- `src/app/pages/` - page components for login, register, dashboard, courses, students, enrollments
- `src/app/app.routes.ts` - client-side routing
- `src/app/app.config.ts` - HTTP client configuration and application bootstrap

---

## Prerequisites

Install the following before running the project:
- .NET 10 SDK
- Node.js 20+ and npm 11+
- SQL Server instance for the backend database
- Optional: Angular CLI for local frontend development

---

## Setup

### 1. Backend configuration

Create local configuration from the example:

```powershell
cd "LearningApi/backend"
copy appsettings.json.example appsettings.json
```

Then update the local file with your SQL Server connection string and JWT values.

> Do not commit `LearningApi/backend/appsettings.json` or any real secret config to GitHub.

### 2. Restore backend dependencies

```powershell
cd "LearningApi/backend"
dotnet restore
```

### 3. Restore frontend dependencies

```powershell
cd "LearningApi/frontend"
npm install
```

---

## Running the application

### Start backend

```powershell
cd "LearningApi/backend"
dotnet run
```

The backend starts on the configured port and exposes Swagger when running in development.

### Start frontend

```powershell
cd "LearningApi/frontend"
npm start
```

Open the browser at `http://localhost:4200`.

### Recommended development flow

1. Start the backend first.
2. Start the frontend next.
3. Use the Angular app to authenticate and call the API.

---

## API and client details

### Backend API routes

- `POST /api/register`
- `POST /api/login`
- `GET /api/students`
- `GET /api/students/{id}`
- `POST /api/students`
- `PUT /api/students/{id}`
- `DELETE /api/students/{id}`
- `GET /api/courses`
- `POST /api/courses`
- `PUT /api/courses/{id}`
- `DELETE /api/courses/{id}`
- `GET /api/enrollments`
- `POST /api/enrollments`
- `DELETE /api/enrollments/{id}`

### Frontend behavior

- User login is stored in browser local storage as JWT token
- Auth interceptor attaches token to API requests
- Search, pagination, and CRUD operations are implemented for courses, students, and enrollments

---

## Commands

### Backend

```powershell
cd "LearningApi/backend"

dotnet restore

dotnet run
```

### Frontend

```powershell
cd "LearningApi/frontend"
npm install
npm start
```

### Build

```powershell
cd "LearningApi/frontend"
npm run build
```

### Test frontend

```powershell
cd "LearningApi/frontend"
ng test
```

---

## Repository layout

```text
LearningApi/
  backend/
    Controllers/
    DTOs/
    Exceptions/
    Helpers/
    Middleware/
    Models/
    Repositories/
    Services/
    appsettings.json.example
    LearningApi.csproj
    Program.cs
  frontend/
    src/
    package.json
    tsconfig.json
    angular.json
  docs/
    PROJECT_GRAPH.md
.gitignore
README.md
```

---

## Notes

- `LearningApi/backend/appsettings.json` is intentionally ignored.
- Keep sensitive data out of Git and use `appsettings.json.example` as the public template.
- The codebase includes generated docs in `LearningApi/docs/PROJECT_GRAPH.md` for architecture reference.

---

## Contact

If you need to extend this project, start by reviewing `LearningApi/backend/Program.cs` and `LearningApi/frontend/src/app/app.routes.ts`.
