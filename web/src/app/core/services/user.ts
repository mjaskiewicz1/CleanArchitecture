import { Injectable } from '@angular/core';
import { Api } from './api';
import { LoginRequest } from '../models/users/login-request';
import { LoginResponse } from '../models/users/login-response';

@Injectable({ providedIn: 'root' })
export class UserService extends Api {
  login(request: LoginRequest) {
    return this.post<LoginResponse>('user/login', request);
  }
}
