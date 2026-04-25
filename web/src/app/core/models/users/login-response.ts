export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt?: string;
  expiresAtUtc?: string;
}
