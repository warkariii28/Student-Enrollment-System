import { Component, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

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
  readonly students = signal<Student[]>([]);
  readonly courses = signal<Course[]>([]);
  readonly enrollments = signal<Enrollment[]>([]);
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

  constructor(
    private readonly studentService: StudentService,
    private readonly courseService: CourseService,
    private readonly enrollmentService: EnrollmentService,
    private readonly authService: AuthService,
    private readonly toast: ToastService,
    private readonly router: Router,
    private readonly confirm: ConfirmService
  ) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.error.set('');

    forkJoin({
      students: this.studentService.getStudents(),
      courses: this.courseService.getCourses(),
      enrollments: this.enrollmentService.getEnrollments()
    }).subscribe({
      next: ({ students, courses, enrollments }) => {
        this.students.set(students);
        this.courses.set(courses);
        this.enrollments.set(enrollments);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load dashboard data. Check login token and backend API.');
      }
    });
  }

  async deleteStudent(student: Student): Promise<void> {
    const ok = await this.confirm.ask(`Delete ${student.name}?`);
    if (!ok) return;

    this.studentService.deleteStudent(student.studentID).subscribe({
      next: () => {
        this.toast.success('Student deleted');
        this.loadDashboard();
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
