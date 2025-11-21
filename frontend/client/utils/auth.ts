import { apiCall } from "./apiCall";

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  fullName: string;
}

export interface AuthResponse {
  accessToken: string;
  user: {
    id: string;
    email: string;
    fullName: string;
  };
}

export const AuthApi = {
  login: (data: LoginDto) => apiCall.post<AuthResponse>("/auth/login", data),
  register: (data: RegisterDto) =>
    apiCall.post<AuthResponse>("/auth/register", data),
  me: () => apiCall.get<AuthResponse>("/auth/me"),
};
