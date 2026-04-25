import { Injectable, signal } from '@angular/core';
import { LoginResponse } from '../models/users/login-response';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const EXPIRES_AT_KEY = 'expiresAt';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  readonly accessToken = signal<string | null>(localStorage.getItem(ACCESS_TOKEN_KEY));
  readonly refreshToken = signal<string | null>(localStorage.getItem(REFRESH_TOKEN_KEY));
  readonly expiresAt = signal<string | null>(localStorage.getItem(EXPIRES_AT_KEY));

  setSession(tokens: LoginResponse): void {
    this.accessToken.set(tokens.accessToken);
    this.refreshToken.set(tokens.refreshToken);
    this.expiresAt.set(tokens.expiresAt ?? tokens.expiresAtUtc ?? null);

    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);

    const expiresAt = tokens.expiresAt ?? tokens.expiresAtUtc;
    if (expiresAt) {
      localStorage.setItem(EXPIRES_AT_KEY, expiresAt);
      return;
    }

    localStorage.removeItem(EXPIRES_AT_KEY);
  }

  clearSession(): void {
    this.accessToken.set(null);
    this.refreshToken.set(null);
    this.expiresAt.set(null);
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
  }
}
