import { Component, OnInit, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

import { Course } from '../../core/models/course';
import { Student } from '../../core/models/student';
import { AuthService } from '../../core/services/auth.service';
import { CourseService } from '../../core/services/course.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { StudentService } from '../../core/services/student.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-enrollment-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './enrollment-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EnrollmentForm implements OnInit {

  // ✅ inject where needed for signals
  private readonly fb = inject(FormBuilder);
  private readonly studentService = inject(StudentService);
  private readonly courseService = inject(CourseService);

  constructor(
    public readonly authService: AuthService,
    private readonly router: Router,
    /* private readonly courseService: CourseService, */
    private readonly toast: ToastService,
    private readonly enrollmentService: EnrollmentService
  ) { }

  // ✅ reactive state
  readonly students = toSignal(this.studentService.students$, { initialValue: [] });
  readonly courses = toSignal(this.courseService.courses$, { initialValue: [] });

  error = '';
  isSubmitting = false;
  isLoadingOptions = true;

  enrollmentForm = this.fb.nonNullable.group({
    studentID: [0, [Validators.required, Validators.min(1)]],
    courseID: [0, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {

    // ✅ fetch only if needed
    if (!this.studentService.hasStudents()) {
      this.studentService.fetchStudents().subscribe();
    }
    this.isLoadingOptions = false;
    if (!this.courseService.hasCourses()) {
      this.courseService.fetchCourses().subscribe();
    }
  }

  // ✅ loading handled once (no nested subscriptions)
  /*     forkJoin({
        courses: this.courseService.getCourses()
      }).subscribe({
        next: () => {
          this.isLoadingOptions = false;
        },
        error: () => {
          this.error = 'Could not load students and courses.';
          this.isLoadingOptions = false;
        }
      }); */


  submit(): void {
    if (this.enrollmentForm.invalid) {
      this.enrollmentForm.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;

    this.enrollmentService
      .addEnrollment(this.enrollmentForm.getRawValue())
      .pipe(finalize(() => this.isSubmitting = false))
      .subscribe({
        next: () => {
          this.toast.success('Enrollment created');
          this.router.navigate(['/dashboard/enrollments']);
        },
        error: (err) => {
          const msg = err.error?.message || 'Could not save enrollment';
          this.toast.error(msg);
          this.error = msg;
        }
      });
  }
}