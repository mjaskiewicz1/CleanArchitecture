import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { form, required, email, FormRoot, FormField } from '@angular/forms/signals';
import { Subject, switchMap, catchError, of, tap } from 'rxjs';
import { LoginRequest } from '../../../core/models/users/login-request';
import { ApiError } from '../../../core/models/users/api-error';
import { UserService } from '../../../core/services/user';

@Component({
  selector: 'app-login',
  imports: [FormRoot, FormField],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly login$ = new Subject<LoginRequest>();

  readonly loginError = signal<string | null>(null);
  readonly model = signal<LoginRequest>({ login: '', password: '' });

  readonly loginForm = form(this.model, (p) => {
    required(p.login);
    email(p.login);
    required(p.password);
  }, {
    submission: { action: async () => { this.onLogin(); } }
  });

  readonly loginResult = toSignal(
    this.login$.pipe(
      switchMap((req) =>
        this.userService.login(req).pipe(
          tap(() => this.router.navigate(['/dashboard'])),
          catchError((err: ApiError) => {
            this.loginError.set(err.detail ?? 'Błąd logowania');
            return of(null);
          })
        )
      )
    ),
    { initialValue: null }
  );

  onLogin(): void {
    this.loginError.set(null);
    this.login$.next(this.model());
  }
}
