import { Component,inject } from '@angular/core';
import { RouterOutlet,Router } from '@angular/router';

import { Navbar } from './shared/navbar/navbar';
import { Sidebar } from './shared/sidebar/sidebar';
import { CommonModule } from '@angular/common';
import { Toast } from './shared/toast/toast';
/* import { LoaderComponent } from './shared/loader/loader'; */
import { ConfirmComponent } from './shared/confirm/confirm';

import { GlobalLoader } from './core/components/global-loader/global-loader';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, Sidebar, Toast, CommonModule /* LoaderComponent */, ConfirmComponent,GlobalLoader ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private router = inject(Router);

  showNavbar(): boolean {
    const url = this.router.url;
    return !url.includes('login') && !url.includes('register');
  }

  showSidebar(): boolean {
    const url = this.router.url;
    return !url.includes('login') && !url.includes('register');
  }
}
