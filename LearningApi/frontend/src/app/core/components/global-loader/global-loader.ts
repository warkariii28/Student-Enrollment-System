import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';

import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-global-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './global-loader.html',
  styleUrls: ['./global-loader.css']
})
export class GlobalLoader {

  private loadingService = inject(LoadingService);

  loading = toSignal(this.loadingService.loading$, { initialValue: false });
}