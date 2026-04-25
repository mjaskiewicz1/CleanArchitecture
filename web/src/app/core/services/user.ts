import { Injectable } from '@angular/core';
import { Api } from './api';
import { LoginRequest } from '../models/users/login-request';
import { LoginResponse } from '../models/users/login-response';
import { RefreshTokenRequest } from '../models/users/refresh-token-request';
import { UserDetailsResponse } from '../models/users/user-details-response';

@Injectable({ providedIn: 'root' })
export class UserService extends Api {
  login(request: LoginRequest) {
    return this.post<LoginResponse>('user/login', request);
  }

  refreshToken(request: RefreshTokenRequest) {
    return this.post<LoginResponse>('user/refresh-token', request);
  }

  getMe() {
    return this.http.get<UserDetailsResponse>(`${this.baseUrl}/user/me`);
  }
}
