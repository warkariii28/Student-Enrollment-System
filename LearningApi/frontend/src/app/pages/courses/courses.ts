import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course';
import { AuthService } from '../../core/services/auth.service';
import { CourseService } from '../../core/services/course.service';
import { ToastService } from '../../core/services/toast.service';
import { ConfirmService } from '../../core/services/confirm.service';
import { SkeletonTableComponent } from '../../core/components/skeleton-table/skeleton-table';

@Component({
  selector: 'app-courses',
  imports: [RouterLink, CurrencyPipe, SkeletonTableComponent],
  templateUrl: './courses.html'
})
export class Courses implements OnInit {
  private readonly courseService = inject(CourseService);
  readonly courses = toSignal(this.courseService.courses$, { initialValue: [] });
  readonly error = signal('');
  readonly loading = signal(true);
  readonly query = signal('');
  readonly page = signal(1);
  readonly pageSize = signal(5);



  readonly filteredCourses = computed(() => {
    const q = this.query().toLowerCase().trim();
    if (!q) return this.courses();

    return this.courses().filter(c =>
      c.courseName.toLowerCase().includes(q)
    );
  });

  readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filteredCourses().length / this.pageSize()))
  );

  readonly pagedCourses = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredCourses().slice(start, start + this.pageSize());
  });

  constructor(
    public readonly authService: AuthService,
    private readonly toast: ToastService,
    private readonly confirm: ConfirmService,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.loading.set(true);
    this.error.set('');

    this.courseService.fetchCourses(true).subscribe({
      next: () => {
        this.loading.set(false);
        this.page.set(1);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load courses. Check backend API and authentication.');
      }
    });
  }

  async deleteCourse(course: Course): Promise<void> {
    const ok = await this.confirm.ask(`Delete ${course.courseName}?`);
    if (!ok) return;

    this.courseService.deleteCourse(course.courseID).subscribe({
      next: () => {
        this.toast.success('Course deleted');
        this.loadCourses();
      },
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
