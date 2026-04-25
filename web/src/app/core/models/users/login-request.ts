export interface LoginRequest extends Record<string, unknown> {
  email: string;
  password: string;
}
