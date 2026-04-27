import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { CourseForm } from './pages/course-form/course-form';
import { Courses } from './pages/courses/courses';
import { Dashboard } from './pages/dashboard/dashboard';
import { EnrollmentForm } from './pages/enrollment-form/enrollment-form';
import { Enrollments } from './pages/enrollments/enrollments';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { StudentForm } from './pages/student-form/student-form';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'dashboard/add', component: StudentForm, canActivate: [authGuard] },
  { path: 'dashboard/edit/:id', component: StudentForm, canActivate: [authGuard] },
  { path: 'dashboard/courses', component: Courses, canActivate: [authGuard] },
  { path: 'dashboard/courses/add', component: CourseForm, canActivate: [authGuard] },
  { path: 'dashboard/courses/edit/:id', component: CourseForm, canActivate: [authGuard] },
  { path: 'dashboard/enrollments', component: Enrollments, canActivate: [authGuard] },
  { path: 'dashboard/enrollments/add', component: EnrollmentForm, canActivate: [authGuard] },
  { path: '**', redirectTo: 'login' }
];
