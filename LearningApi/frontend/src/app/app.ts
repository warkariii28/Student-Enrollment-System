import { Component,inject } from '@angular/core';
import { RouterOutlet,Router } from '@angular/router';

import { Footer } from './shared/footer/footer';
import { Navbar } from './shared/navbar/navbar';
import { CommonModule } from '@angular/common';
import { Toast } from './shared/toast/toast';
import { LoaderComponent } from './shared/loader/loader';
import { ConfirmComponent } from './shared/confirm/confirm';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, Footer,Toast,CommonModule,LoaderComponent,ConfirmComponent ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private router = inject(Router);

  showNavbar(): boolean {
    const url = this.router.url;
    return !url.includes('login') && !url.includes('register');
  }
}
