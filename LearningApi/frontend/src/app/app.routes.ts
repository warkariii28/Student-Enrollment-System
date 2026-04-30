import { Routes } from '@angular/router';
import { roleGuard } from './core/guards/role.guard';
import { authGuard } from './core/guards/auth.guard';
import { CourseForm } from './pages/course-form/course-form';
import { Courses } from './pages/courses/courses';
import { Dashboard } from './pages/dashboard/dashboard';
import { EnrollmentForm } from './pages/enrollment-form/enrollment-form';
import { Enrollments } from './pages/enrollments/enrollments';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { StudentForm } from './pages/student-form/student-form';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout';
import { AppLayoutComponent } from './layouts/app-layout/app-layout';

export const routes: Routes = [

  // Redirect root
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  // AUTH LAYOUT
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: Login },
      { path: 'register', component: Register }
    ]
  },

  {
    path: 'dashboard',
    component: AppLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', component: Dashboard },

      // ADMIN ONLY
      {
        path: 'add',
        component: StudentForm,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'edit/:id',
        component: StudentForm,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },

      // STUDENT + ADMIN 
      {
        path: 'courses',
        component: Courses
      },
      {
        path: 'courses/add',
        component: CourseForm,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'courses/edit/:id',
        component: CourseForm,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },

      // VIEW ALLOWED
      {
        path: 'enrollments',
        component: Enrollments
      },
      // ADD = ONLY ADMIN
      {
        path: 'enrollments/add',
        component: EnrollmentForm,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      }
    ]
  },

  // Fallback
  { path: '**', redirectTo: 'login' }
];
