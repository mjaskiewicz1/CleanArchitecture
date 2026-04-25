import { Component, inject, signal, WritableSignal } from '@angular/core';
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
  templateUrl: './login.html',
})
export class Login implements ErrorTranslatable {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly login$ = new Subject<LoginRequest>();

  readonly loginError = signal<string | null>(null);
  readonly model = signal<LoginRequest>({ email: '', password: '' });

  private readonly fieldServerErrors: Record<keyof LoginRequest, WritableSignal<string | null>> = {
    email: signal(null),
    password: signal(null),
  };

  private readonly errorTranslations: Record<string, string> = {
    'Invalid email or password.': 'Nieprawidłowy email lub hasło',
    'One or more validation errors occurred': 'Wystąpiły błędy walidacji',
    "'Email' is not a valid email address.": 'Nieprawidłowy adres email',
    "'Email' must not be empty.": 'Nieprawidłowy adres email',
    "'Password' must not be empty.": 'Hasło jest wymagane',
  };

  translateError(error: string): string {
    return this.errorTranslations[error] ?? error;
  }

  readonly loginForm = form(
    this.model,
    (p) => {
      required(p.email, { message: "'Email' must not be empty." });
      email(p.email, { message: "'Email' is not a valid email address." });
      required(p.password, { message: "'Password' must not be empty." });
    },
    {
      submission: { action: async () => this.onLogin() },
    },
  );

  getFieldError(field: keyof LoginRequest): string | null {
    // Server error takes priority over client-side validation error
    const serverError = this.fieldServerErrors[field]();
    if (serverError) return serverError;

    const ctrl = this.loginForm[field]();
    if (!ctrl.touched() || ctrl.valid()) return null;

    const errors = ctrl.errors();
    if (errors.length === 0) return null;

    return this.translateError(errors[0].message ?? errors[0].kind);
  }

  private readonly loginResult = toSignal(
    this.login$.pipe(
      switchMap((req) =>
        this.userService.login(req).pipe(
          tap((response) => {
            localStorage.setItem('jwt', response.token);
          void  this.router.navigate(['/dashboard']);
          }),
          catchError((err: ApiError) => {
            this.handleApiError(err);
            return of(null);
          }),
        ),
      ),
    ),
    { initialValue: null },
  );

  onLogin(): void {
    this.loginError.set(null);
    Object.values(this.fieldServerErrors).forEach((s) => s.set(null));
    this.login$.next(this.model());
  }

  private handleApiError(err: ApiError): void {
    if (err.errors && Object.keys(err.errors).length > 0) {
      for (const [key, messages] of Object.entries(err.errors)) {
        const field = key.toLowerCase() as keyof LoginRequest;

        if (field in this.model()) {
          this.fieldServerErrors[field].set(this.translateError(messages[0]));
          continue;
        }
        this.loginError.set(this.translateError(messages[0]));
      }
      return;
    }
    this.loginError.set(this.translateError(err.detail || 'Błąd logowania'));
  }
}
