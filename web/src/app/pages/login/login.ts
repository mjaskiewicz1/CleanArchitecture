import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { form, required, email, FormRoot, FormField } from '@angular/forms/signals';
import { Subject, switchMap, catchError, of, tap } from 'rxjs';
import { LoginRequest } from '../../core/models/users/login-request';
import { ApiError } from '../../core/models/users/api-error';
import { UserService } from '../../core/services/user';
import { ErrorTranslatable } from '../../core/interfaces/error-translatable.interface';

@Component({
  selector: 'app-login',
  imports: [FormRoot, FormField],
  templateUrl: './login.html'
})
export class Login implements ErrorTranslatable {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly login$ = new Subject<LoginRequest>();

  readonly loginError = signal<string | null>(null);
  readonly model = signal<LoginRequest>({ email: '', password: '' });

  translateError(error: string): string {
    const translations: Record<string, string> = {
      'Invalid email or password.': 'Nieprawidłowy email lub hasło',
      'One or more validation errors occurred': 'Wystąpiły błędy walidacji',
      "'Email' is not a valid email address.": 'Nieprawidłowy adres email',
      "'Email' must not be empty.": 'Nieprawidłowy adres email',
      "'Password' must not be empty.": 'Hasło jest wymagane'
    };
    return translations[error] || error;
  }

  readonly loginForm = form(this.model, (p) => {
    required(p.email, { message: "'Email' must not be empty." });
    email(p.email, { message: "'Email' is not a valid email address." });
    required(p.password, { message: "'Password' must not be empty." });
  }, {
    submission: { action: async () => { this.onLogin(); } }
  });

  getFieldError(field: 'email' | 'password'): string | null {
    const fieldControl = this.loginForm[field]();
    if (!fieldControl.touched() || fieldControl.valid()) return null;

    const errors = fieldControl.errors();
    if (errors.length === 0) return null;

    return this.translateError(errors[0].message || errors[0].kind);
  }

  readonly loginResult = toSignal(
    this.login$.pipe(
      switchMap((req) =>
        this.userService.login(req).pipe(
          tap(() => this.router.navigate(['/dashboard'])),
          catchError((err: ApiError) => {
            this.handleApiError(err);
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

  private handleApiError(err: ApiError): void {
    if (err.errors && Object.keys(err.errors).length > 0) {
      const firstError = Object.values(err.errors)[0][0];
      this.loginError.set(this.translateError(firstError));
    } else {
      this.loginError.set(this.translateError(err.detail || 'Błąd logowania'));
    }
  }
}
