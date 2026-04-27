export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  passwordHash: string;
}

export interface LoginResponse {
  token: string;
}
