import { Component, ChangeDetectionStrategy, ChangeDetectorRef, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { ValidationErrors } from '../../core/models/api-response';
import { getFieldError } from '../../core/utils/validation-errors';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  private readonly fb = inject(FormBuilder);

  error = '';
  isSubmitting = false;

  validationErrors: ValidationErrors | null = null;
  readonly getFieldError = getFieldError;

  loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly toast: ToastService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  submit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.validationErrors = null;

    this.error = '';
    this.isSubmitting = true;
    this.cdr.markForCheck(); // ✅ trigger update safely

    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.cdr.markForCheck(); // ✅ safe update
        }),
      )
      .subscribe({
        next: () => {
          this.toast.success('Login successful');
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.validationErrors = err.error?.data ?? null;
          this.error = err.error?.message || 'Login failed';
          this.toast.error(this.error);
          this.cdr.markForCheck();
        },
      });
  }
}
