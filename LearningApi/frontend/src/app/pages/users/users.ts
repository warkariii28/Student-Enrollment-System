import { Component, OnInit, signal } from '@angular/core';

import { User } from '../../core/models/user';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { PageHeaderComponent } from '../../shared/page-header/page-header';
import { SkeletonTableComponent } from '../../core/components/skeleton-table/skeleton-table';

@Component({
  selector: 'app-users',
  imports: [PageHeaderComponent, SkeletonTableComponent],
  templateUrl: './users.html',
})
export class Users implements OnInit {
  readonly users = signal<User[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');

  constructor(
    private readonly authService: AuthService,
    private readonly toast: ToastService,
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.error.set('');

    this.authService.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load users.');
        this.loading.set(false);
      },
    });
  }

  changeRole(user: User, role: string): void {
    if (user.role === role) return;

    this.authService.assignRole(user.userId, role).subscribe({
      next: (message) => {
        this.toast.success(message || 'Role updated');

        this.users.update((users) =>
          users.map((u) => (u.userId === user.userId ? { ...u, role } : u)),
        );
      },
      error: (err) => {
        const message = err.error?.message || 'Could not update role';
        this.toast.error(message);
        this.loadUsers();
        this.error.set(message);
      },
    });
  }
}
