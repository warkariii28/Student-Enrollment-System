export interface Enrollment {
  enrollmentID: number;
  studentName: string;
  courseName: string;
  enrollmentDate: string;
}

export interface EnrollmentPayload {
  studentID: number;
  courseID: number;
}
