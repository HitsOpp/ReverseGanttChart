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
  token: string;
}

export const AuthApi = {
  login: (data: LoginDto) => apiCall.post<AuthResponse>("/login", data),
  register: (data: RegisterDto) =>
    apiCall.post<AuthResponse>("/register", data),
  me: () => apiCall.get<AuthResponse>("/Auth/me"),
};
