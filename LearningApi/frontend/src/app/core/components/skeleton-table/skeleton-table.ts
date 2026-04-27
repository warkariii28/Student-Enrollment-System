import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './skeleton-table.html',
  styleUrls: ['./skeleton-table.css']
})
export class SkeletonTableComponent {
  @Input() rows = 6;
  @Input() cols = 5;

  get rowArray() {
    return Array(this.rows);
  }

  get colArray() {
    return Array(this.cols);
  }
}