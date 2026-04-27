export interface Course {
  courseID: number;
  courseName: string;
  fee: number;
  durationWeeks: number;
}

export interface CoursePayload {
  courseName: string;
  fee: number;
  durationWeeks: number;
}
