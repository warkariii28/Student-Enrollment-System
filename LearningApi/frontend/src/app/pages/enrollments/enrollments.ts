import { DatePipe } from '@angular/common';
import { Component, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { ConfirmService } from '../../core/services/confirm.service';
import { Enrollment } from '../../core/models/enrollment';
import { SkeletonTableComponent } from '../../core/components/skeleton-table/skeleton-table';

@Component({
  selector: 'app-enrollments',
  imports: [RouterLink, DatePipe, SkeletonTableComponent],
  templateUrl: './enrollments.html'
})
export class Enrollments implements OnInit {
  readonly enrollments = signal<Enrollment[]>([]);
  readonly error = signal('');
  readonly loading = signal(true);

  // 🔍 Search + Pagination
  readonly query = signal('');
  readonly page = signal(1);
  readonly pageSize = signal(5);

  // ✅ Filtered data
  readonly filtered = computed(() => {
    const q = this.query().toLowerCase().trim();
    if (!q) return this.enrollments();

    return this.enrollments().filter(e =>
      e.studentName.toLowerCase().includes(q) ||
      e.courseName.toLowerCase().includes(q)
    );
  });

  // ✅ Total pages
  readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filtered().length / this.pageSize()))
  );

  // ✅ Paginated data
  readonly paged = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filtered().slice(start, start + this.pageSize());
  });

  constructor(
    private readonly enrollmentService: EnrollmentService,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly confirm: ConfirmService
  ) {}

  ngOnInit(): void {
    this.loadEnrollments();
  }

  loadEnrollments(): void {
    this.loading.set(true);
    this.error.set('');

    this.enrollmentService.getEnrollments().subscribe({
      next: (enrollments) => {
        this.enrollments.set(enrollments);
        this.loading.set(false);
        this.page.set(1); // ✅ reset page after reload
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load enrollments. Check backend API and authentication.');
      }
    });
  }

  // 🔍 Search handler
  setQuery(value: string): void {
    this.query.set(value);
    this.page.set(1);
  }

  // ⬅️➡️ Pagination
  nextPage(): void {
    if (this.page() < this.totalPages()) {
      this.page.update(p => p + 1);
    }
  }

  prevPage(): void {
    if (this.page() > 1) {
      this.page.update(p => p - 1);
    }
  }

  async deleteEnrollment(enrollment: Enrollment): Promise<void> {
    const ok = await this.confirm.ask(
      `Remove ${enrollment.studentName} from ${enrollment.courseName}?`
    );
    if (!ok) return;

    this.enrollmentService.deleteEnrollment(enrollment.enrollmentID).subscribe({
      next: () => this.loadEnrollments()
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}