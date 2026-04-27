import { Component, OnInit, inject, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';

import { Course } from '../../core/models/course';
import { Student } from '../../core/models/student';
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
  private readonly fb = inject(FormBuilder);

  error = '';
  isSubmitting = false;
  isLoadingOptions = true;
  students: Student[] = [];
  courses: Course[] = [];

  enrollmentForm = this.fb.nonNullable.group({
    studentID: [0, [Validators.required, Validators.min(1)]],
    courseID: [0, [Validators.required, Validators.min(1)]]
  });

  constructor(
    private readonly router: Router,
    private readonly studentService: StudentService,
    private readonly courseService: CourseService,
    private readonly toast: ToastService,
    private readonly enrollmentService: EnrollmentService,
    private readonly cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    forkJoin({
      students: this.studentService.getStudents(),
      courses: this.courseService.getCourses()
    }).subscribe({
      next: ({ students, courses }) => {
        this.students = students;
        this.courses = courses;
        this.isLoadingOptions = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Could not load students and courses for enrollment.';
        this.isLoadingOptions = false;
        this.cdr.markForCheck();
      }
    });
  }

  submit(): void {
    if (this.enrollmentForm.invalid) {
      this.enrollmentForm.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;

    this.enrollmentService
      .addEnrollment(this.enrollmentForm.getRawValue())
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.cdr.markForCheck();  
        })
      )
      .subscribe({
        next: () => {
          this.toast.success('Enrollment created');
          this.router.navigate(['/dashboard/enrollments']);
        },
        error: (err) => {
          const msg = err.error?.message || 'Could not save enrollment';
          this.toast.error(msg);
          this.error = msg;
          this.cdr.markForCheck();
        }
      });
  }
}

