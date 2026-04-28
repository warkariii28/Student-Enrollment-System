import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html'
})
export class Register {
  private readonly fb = inject(FormBuilder);

  error = '';
  isSubmitting = false;

  registerForm = this.fb.nonNullable.group({
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly toast: ToastService
  ) { }

  submit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.error = '';
    this.isSubmitting = true;
    this.authService
      .register(this.registerForm.getRawValue())
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.toast.success('Registration successful');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          const msg = err.error?.message || 'Registration failed';
          this.toast.error(msg);
          this.error = msg;
        }
      });
  }
}
