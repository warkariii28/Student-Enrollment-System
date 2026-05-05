# LearningApi Frontend

This is the Angular frontend for the LearningApi project. It provides the login/register screens, protected dashboard layout, student management, course catalog, and enrollment views.

## Tech Stack

- Angular 21
- TypeScript
- RxJS
- Angular signals
- Angular router guards and interceptors
- lucide-angular icons
- CSS custom properties for design tokens

## Prerequisites

- Node.js
- npm
- Backend API running at `http://localhost:5140`

## Install

```bash
npm install
```

## Run Development Server

```bash
npm start
```

Open:

```txt
http://localhost:4200
```

## Build

```bash
npm run build
```

Build output is created in:

```txt
frontend/dist/frontend
```

## Test

```bash
npm test
```

## API Configuration

The API base URL is configured in:

```txt
src/environments/environment.ts
```

Current development value:

```ts
export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5140'
};
```

## Main Routes

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

## Important Folders

```txt
src/app/core/          Models, services, guards, interceptors, shared utilities
src/app/layouts/       Auth layout and protected app layout
src/app/pages/         Login, register, dashboard, forms, lists
src/app/shared/        Navbar, sidebar, footer, toast, confirm, loader, page header
src/styles/            Global CSS tokens, base styles, layout, components
```

## Frontend Features

- Authenticated app shell with navbar and sidebar
- JWT token attachment through an auth interceptor
- Protected routes through `authGuard`
- Admin-only routes through `roleGuard`
- Reusable page header for dashboard-style pages
- Consistent table toolbar with search and refresh
- Pagination for list screens
- Toast notifications and confirm dialogs
- Shared design tokens in `src/styles/tokens.css`

## Notes For Beginners

Start by reading these files:

```txt
src/app/app.routes.ts
src/app/core/services/auth.service.ts
src/app/core/interceptors/auth.interceptor.ts
src/app/pages/dashboard/dashboard.ts
src/app/shared/page-header/page-header.ts
```

These show how routing, authentication, API calls, signals, and reusable UI pieces connect together.
