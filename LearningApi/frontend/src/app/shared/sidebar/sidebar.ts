import { Component, computed, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {
  readonly isSidebarOpen = signal(false);
  readonly isLoggedIn = computed(() => this.authService.isLoggedIn());

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) { }

  toggleSidebar(): void {
    this.isSidebarOpen.update((isOpen) => !isOpen);
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
  }

  navigateTo(route: string): void {
    this.closeSidebar();
    this.router.navigate([route]);
  }
}
