import { Component, computed, signal, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { StudentService } from '../../core/services/student.service';
import { CourseService } from '../../core/services/course.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { LucideAngularModule, Users, BookOpen, ClipboardList, UserPlus, Plus } from 'lucide-angular';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule, LucideAngularModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})

export class Sidebar implements OnInit, OnDestroy {
  private readonly sidebarOpenClass = 'sidebar-open';
  private readonly body = document.body;
  private readonly routerSubscriptions = new Subscription();

  readonly isSidebarOpen = signal(false);
  readonly isLoggedIn = computed(() => this.authService.isLoggedIn());
  students = signal<any[]>([]);
  courses = signal<any[]>([]);
  enrollments = signal<any[]>([]);
  readonly Users = Users;
  readonly BookOpen = BookOpen;
  readonly ClipboardList = ClipboardList;
  readonly UserPlus = UserPlus;
  readonly Plus = Plus;

  constructor(
    private studentService: StudentService,
    private courseService: CourseService,
    private enrollmentService: EnrollmentService,
    private authService: AuthService,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.studentService.getStudents().subscribe(data => this.students.set(data));
    this.courseService.getCourses().subscribe(data => this.courses.set(data));
    this.enrollmentService.getEnrollments().subscribe(data => this.enrollments.set(data));

    this.routerSubscriptions.add(
      this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe(() => this.closeSidebar())
    );
  }

  ngOnDestroy(): void {
    this.routerSubscriptions.unsubscribe();
    this.setBodyClass(false);
  }

  toggleSidebar(): void {
    this.isSidebarOpen.update((isOpen) => {
      const nextOpen = !isOpen;
      this.setBodyClass(nextOpen);
      return nextOpen;
    });
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
    this.setBodyClass(false);
  }

  private setBodyClass(open: boolean): void {
    this.body.classList.toggle(this.sidebarOpenClass, open);
  }

  navigateTo(route: string): void {
    this.closeSidebar();
    this.router.navigate([route]);
  }

  logout(): void {
    this.authService.logout();
  }
}
