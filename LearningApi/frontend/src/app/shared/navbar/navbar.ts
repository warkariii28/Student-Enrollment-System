import { Component, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
  readonly isLoggedIn = computed(() => this.authService.isLoggedIn());

  readonly currentUser = computed(() => this.authService.getUser());

  readonly username = computed(() => {
    const user = this.currentUser();

    return user?.username || user?.name || user?.email || 'User';
  });
  
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) { }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}