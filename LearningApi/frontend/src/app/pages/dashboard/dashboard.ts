import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

import { Course } from '../../core/models/course';
import { Enrollment } from '../../core/models/enrollment';
import { Student } from '../../core/models/student';

import { AuthService } from '../../core/services/auth.service';
import { CourseService } from '../../core/services/course.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { StudentService } from '../../core/services/student.service';
import { ConfirmService } from '../../core/services/confirm.service';
import { ToastService } from '../../core/services/toast.service';

import { SkeletonTableComponent } from '../../core/components/skeleton-table/skeleton-table';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, SkeletonTableComponent],
  templateUrl: './dashboard.html'
})
export class Dashboard implements OnInit {

  // ✅ inject() used correctly
  private readonly studentService = inject(StudentService);
  private readonly courseService = inject(CourseService);
  private readonly enrollmentService = inject(EnrollmentService);

  // other services via constructor (clean separation)
  constructor(
    public readonly authService: AuthService,
    private readonly toast: ToastService,
    private readonly router: Router,
    private readonly confirm: ConfirmService
  ) { }

  // ✅ state from service
  readonly students = toSignal(this.studentService.students$, { initialValue: [] });

  readonly courses = toSignal(this.courseService.courses$, { initialValue: [] });
  readonly enrollments = toSignal(this.enrollmentService.enrollments$, { initialValue: [] });
  readonly error = signal('');
  readonly loading = signal(true);

  readonly query = signal('');
  readonly page = signal(1);
  readonly pageSize = signal(5);

  readonly filteredStudents = computed(() => {
    const q = this.query().toLowerCase().trim();
    if (!q) return this.students();

    return this.students().filter(s =>
      s.name.toLowerCase().includes(q) ||
      s.email.toLowerCase().includes(q)
    );
  });

  readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filteredStudents().length / this.pageSize()))
  );

  readonly pagedStudents = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredStudents().slice(start, start + this.pageSize());
  });

  ngOnInit(): void {
    this.loadDashboard();
    this.loading.set(false);
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.error.set('');

    // students
    if (!this.studentService.hasStudents()) {
      this.studentService.fetchStudents().subscribe();
    }

    // courses
    if (!this.courseService.hasCourses()) {
      this.courseService.fetchCourses().subscribe();
    }

    // enrollments
    if (!this.enrollmentService.hasEnrollments()) {
      this.enrollmentService.fetchEnrollments().subscribe();
    }

    // ✅ no forkJoin needed anymore
    this.loading.set(false);
  }

  async deleteStudent(student: Student): Promise<void> {
    const ok = await this.confirm.ask(`Delete ${student.name}?`);
    if (!ok) return;

    this.studentService.deleteStudent(student.studentID).subscribe({
      next: () => {
        this.toast.success('Student deleted');

        // ❌ avoid full reload
        // this.loadDashboard();

        // ✅ state already updates → no reload needed
      },
      error: () => this.error.set('Could not delete student')
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  setQuery(value: string) {
    this.query.set(value);
    this.page.set(1);
  }

  nextPage() {
    if (this.page() < this.totalPages()) {
      this.page.update(p => p + 1);
    }
  }

  prevPage() {
    if (this.page() > 1) {
      this.page.update(p => p - 1);
    }
  }
}