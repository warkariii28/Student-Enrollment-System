import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// adjust paths based on your structure
import { Sidebar } from '../../shared/sidebar/sidebar';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-app-layout',
  standalone: true,
  imports: [RouterOutlet, Sidebar, Navbar],
  templateUrl: './app-layout.html'
})
export class AppLayoutComponent {}