export interface AuthResponse {
  token: string;
  user: {
    userId: number;
    name: string;
    email: string;
  };
}