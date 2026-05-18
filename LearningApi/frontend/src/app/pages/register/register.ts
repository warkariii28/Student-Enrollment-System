import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

import { ValidationErrors } from '../../core/models/api-response';
import { getFieldError } from '../../core/utils/validation-errors';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
})
export class Register {
  private readonly fb = inject(FormBuilder);

  error = '';
  isSubmitting = false;

  validationErrors: ValidationErrors | null = null;
  readonly getFieldError = getFieldError;

  registerForm = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly toast: ToastService,
  ) {}

  submit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;
    this.validationErrors = null;

    this.authService.register(this.registerForm.getRawValue()).subscribe({
      next: () => {
        this.toast.success('Registration successful');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.validationErrors = err.error?.data ?? null;
        this.error = err.error?.message || 'Registration failed';
        this.toast.error(this.error);
        this.isSubmitting = false;
      },
    });
  }
}
