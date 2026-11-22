export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
  userId: number;
  email: string;
  role: string;
  mustChangePassword: boolean;
}
