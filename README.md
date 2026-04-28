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

The backend is an ASP.NET Core Web API targeting `net10.0` and uses a layered architecture:
- `Controllers/` - API endpoints that accept DTOs, perform request validation, and return a consistent response envelope
- `Services/` - business logic and auth flow, including JWT generation and password hashing
- `Repositories/` - SQL Server access with a repository pattern that isolates raw ADO.NET calls from the controller logic
- `Models/` - domain models and DTOs used across the API
- `Middleware/` - global exception handling ensures consistent error responses

Backend implementation details:
- `Program.cs` registers all services and repositories with dependency injection and configures `AddCors`, `AddAuthentication`, `AddAuthorization`, and Swagger
- Authentication is implemented with JWTs delivered from `AuthService` and validated by ASP.NET Core middleware
- `AuthService` hashes passwords using `BCrypt.Net-Next`, verifies login credentials, and creates tokens with claims for the username, email, and user ID
- All protected routes require `[Authorize]`; only `POST /api/login` and `POST /api/register` are public
- Controllers convert DTO payloads into domain models, then call services to execute CRUD operations
- `ResponseHelper` wraps responses in a uniform JSON envelope: `success`, `message`, and `data`

### Frontend

The frontend is an Angular 21 application with a reactive client-side state and API integration:
- `app.config.ts` configures Angular HTTP client middleware and router providers
- `app.routes.ts` defines public and protected routes, with `authGuard` blocking unauthenticated access to dashboard and management pages
- `AuthService` executes login/register API calls, stores the JWT in `localStorage`, validates expiry, and provides logout behavior
- `authInterceptor` attaches `Authorization: Bearer {token}` to outgoing API requests and handles HTTP errors centrally
- `loaderInterceptor` toggles UI loading state for network activity

Frontend implementation details:
- The app uses `signal` and `computed` state management in components such as `Courses` for search, pagination, and filtering logic
- Search input and pagination are handled locally in the `Courses` page; changing the query resets the page to `1`
- CRUD operations call backend services in `core/services/` for students, courses, and enrollments
- The `ConfirmService` asks users before destructive actions like deleting a course or enrollment
- The `ToastService` displays success and error notifications for user feedback
- `API_BASE_URL` is centralized in `core/api.config.ts`, making backend URL changes easy

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

### 4. Configure environment URLs

Update the frontend environment files for API endpoints:

- `LearningApi/frontend/src/environments/environment.ts` (development): `apiBaseUrl: 'http://localhost:5140'`
- `LearningApi/frontend/src/environments/environment.prod.ts` (production): Update `apiBaseUrl` to your deployed backend URL

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
- The UI uses a confirm dialog before deleting records and a toast system for success/error feedback
- `authGuard` protects dashboard pages and redirects unauthenticated users to the login page

### Auth and HTTP flow

- `AuthService.login()` sends credentials to `POST /api/login`, saves the returned JWT, and returns the token to the caller
- `authInterceptor` adds the JWT to every outbound request and logs out the user automatically on HTTP `401`
- `loaderInterceptor` manages a loading spinner for all in-flight HTTP requests
- The frontend expects the backend to return a response envelope with `success`, `message`, and `data`

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

This creates a production build using `environment.prod.ts` configuration. The build artifacts are stored in the `dist/` directory.

### Build for production

```powershell
cd "LearningApi/frontend"
npm run build -- --configuration production
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

## Recent Improvements

### Code Quality Enhancements
- **HTTP Status Codes**: All endpoints now return correct REST status codes (201 for POST, 204 for DELETE, 400 for invalid data, 401 for unauthorized)
- **ModelState Validation**: All POST and PUT endpoints include `if (!ModelState.IsValid)` checks with consistent error responses
- **DTO Validation**: Enhanced Create/Update DTOs with proper validation attributes (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`)
- **Custom Exceptions**: Replaced all generic `throw new Exception()` with specific `BadRequestException` and `NotFoundException`
- **Response Consistency**: All API responses now use `ResponseHelper.Success()` and `ResponseHelper.Fail()` for uniform JSON structure
- **Strong Typing**: `AuthResponseDto.User` changed from `object` to strongly-typed `UserDto`

### Architecture Refinements
- **Service Layer**: Removed redundant validation logic from services (validation now handled by DTOs and controllers)
- **Controller Layer**: Ensured all entity inputs use DTOs instead of raw Models, standardized response formats
- **Exception Handling**: Global middleware catches custom exceptions and returns appropriate HTTP status codes
- **Repository Layer**: Maintained data access isolation with proper SQL injection prevention via parameterized queries

### Frontend Configuration
- **Environment Variables**: Updated Angular environment files to properly configure API base URLs for development and production
- **API Configuration**: Refactored `api.config.ts` to dynamically use environment-specific URLs instead of hardcoded values
- **Password Security**: BCrypt hashing for all passwords, no sensitive data in responses
- **Input Validation**: Comprehensive validation at DTO level prevents invalid data from reaching business logic
- **Error Handling**: Consistent error responses prevent information leakage

---

## Notes

- `LearningApi/backend/appsettings.json` is intentionally ignored.
- Keep sensitive data out of Git and use `appsettings.json.example` as the public template.
- The codebase includes generated docs in `LearningApi/docs/PROJECT_GRAPH.md` for architecture reference.

---

## Contact

If you need to extend this project, start by reviewing `LearningApi/backend/Program.cs` and `LearningApi/frontend/src/app/app.routes.ts`.
