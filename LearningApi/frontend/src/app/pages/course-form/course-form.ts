import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { CourseService } from '../../core/services/course.service';

@Component({
  selector: 'app-course-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './course-form.html'
})
export class CourseForm implements OnInit {
  private readonly fb = inject(FormBuilder);

  error = '';
  isSubmitting = false;
  isLoadingCourse = false;
  courseId: number | null = null;

  courseForm = this.fb.nonNullable.group({
    courseName: ['', Validators.required],
    fee: [0, [Validators.required, Validators.min(0)]],
    durationWeeks: [1, [Validators.required, Validators.min(1)]]
  });

  get isEditMode(): boolean {
    return this.courseId !== null;
  }

  constructor(
    public readonly authService: AuthService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly courseService: CourseService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.courseId = Number(id);
    this.isLoadingCourse = true;
    this.courseService.getCourse(this.courseId).subscribe({
      next: (course) => {
        this.courseForm.patchValue(course);
        this.isLoadingCourse = false;
      },
      error: () => {
        this.error = 'Could not load course';
        this.isLoadingCourse = false;
      }
    });
  }

  submit(): void {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;
    const payload = this.courseForm.getRawValue();

    if (this.isEditMode) {
      this.courseService
        .updateCourse(this.courseId!, payload)
        .pipe(finalize(() => (this.isSubmitting = false)))
        .subscribe({
          next: () => this.router.navigate(['/dashboard/courses']),
          error: () => (this.error = 'Could not save course')
        });
      return;
    }

    this.courseService
      .addCourse(payload)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => this.router.navigate(['/dashboard/courses']),
        error: () => (this.error = 'Could not save course')
      });
  }
}
