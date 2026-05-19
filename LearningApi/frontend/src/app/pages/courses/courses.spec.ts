import { BehaviorSubject, of } from 'rxjs';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { Courses } from './courses';
import { AuthService } from '../../core/services/auth.service';
import { CourseService } from '../../core/services/course.service';
import { ConfirmService } from '../../core/services/confirm.service';
import { ToastService } from '../../core/services/toast.service';

describe('Courses', () => {
  let component: Courses;
  let fixture: ComponentFixture<Courses>;

  const coursesSubject = new BehaviorSubject<any[]>([]);
  const totalCountSubject = new BehaviorSubject<number>(0);

  const courseServiceMock = {
    courses$: coursesSubject.asObservable(),
    totalCount$: totalCountSubject.asObservable(),
    fetchCourses: vi.fn(() => of([])),
    deleteCourse: vi.fn(() => of(void 0))
  };

  const authServiceMock = {
    logoutFromServer: vi.fn(() => of(void 0)),
    logout: vi.fn(),
    isAdmin: vi.fn(() => true)
  };

  const toastMock = {
    success: vi.fn(),
    error: vi.fn()
  };

  const confirmMock = {
    ask: vi.fn()
  };

  beforeEach(async () => {
    vi.clearAllMocks();

    coursesSubject.next([]);
    totalCountSubject.next(12);

    await TestBed.configureTestingModule({
      imports: [Courses],
      providers: [
        provideRouter([]),
        { provide: CourseService, useValue: courseServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: ToastService, useValue: toastMock },
        { provide: ConfirmService, useValue: confirmMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Courses);
    component = fixture.componentInstance;
  });

  it('loads first page on init', () => {
    component.ngOnInit();

    expect(courseServiceMock.fetchCourses).toHaveBeenCalledWith(1, 5, '', true);
    expect(component.loading()).toBe(false);
  });

  it('resets to first page and searches with query', () => {
    component.page.set(3);

    component.setQuery('angular');

    expect(component.page()).toBe(1);
    expect(component.query()).toBe('angular');
    expect(courseServiceMock.fetchCourses).toHaveBeenCalledWith(1, 5, 'angular', true);
  });

  it('moves to next page when another page exists', () => {
    component.page.set(1);
    totalCountSubject.next(12);

    component.nextPage();

    expect(component.page()).toBe(2);
    expect(courseServiceMock.fetchCourses).toHaveBeenCalledWith(2, 5, '', true);
  });

  it('does not move to next page when already on last page', () => {
    component.page.set(3);
    totalCountSubject.next(12);

    component.nextPage();

    expect(component.page()).toBe(3);
    expect(courseServiceMock.fetchCourses).not.toHaveBeenCalled();
  });

  it('moves to previous page when page is greater than one', () => {
    component.page.set(2);

    component.prevPage();

    expect(component.page()).toBe(1);
    expect(courseServiceMock.fetchCourses).toHaveBeenCalledWith(1, 5, '', true);
  });

  it('does not move to previous page from first page', () => {
    component.page.set(1);

    component.prevPage();

    expect(component.page()).toBe(1);
    expect(courseServiceMock.fetchCourses).not.toHaveBeenCalled();
  });
});