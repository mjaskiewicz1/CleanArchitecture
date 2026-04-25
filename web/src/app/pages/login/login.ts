import { Component, inject, signal, WritableSignal } from '@angular/core';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { form, required, email, FormRoot, FormField } from '@angular/forms/signals';
import { Subject, switchMap, catchError, of, tap } from 'rxjs';
import { LoginRequest } from '../../core/models/users/login-request';
import { ApiError } from '../../core/models/users/api-error';
import { UserService } from '../../core/services/user';
import { ApiErrorHandler } from '../../core/services/api-error-handler';


@Component({
  selector: 'app-login',
  imports: [FormRoot, FormField],
  templateUrl: './login.html',
})
export class Login {
  private readonly userService     = inject(UserService);
  private readonly router          = inject(Router);
  private readonly apiErrorHandler = inject(ApiErrorHandler);
  private readonly login$          = new Subject<LoginRequest>();

  readonly loginError = signal<string | null>(null);
  readonly model      = signal<LoginRequest>({ email: '', password: '' });

  readonly fieldServerErrors: Record<keyof LoginRequest, WritableSignal<string | null>> = {
    email   : signal(null),
    password: signal(null),
  };

  private readonly errorTranslations: Record<string, string> = {
    'Invalid email or password.'            : 'Nieprawidłowy email lub hasło',
    "'Email' is not a valid email address." : 'Nieprawidłowy adres email',
    "'Email' must not be empty."            : 'Nieprawidłowy adres email',
    "'Password' must not be empty."         : 'Hasło jest wymagane',
  };

  readonly loginForm = form(
    this.model,
    (p) => {
      required(p.email,    { message: "'Email' must not be empty." });
      email(p.email,       { message: "'Email' is not a valid email address." });
      required(p.password, { message: "'Password' must not be empty." });
    },
    {
      submission: { action: async () => this.onLogin() },
    },
  );

  getFieldError(field: keyof LoginRequest): string | null {
    return this.apiErrorHandler.resolveFieldError(
      field as string,
      this.fieldServerErrors,
      this.loginForm[field],
      this.errorTranslations
    );
  }

  private readonly loginResult = toSignal(
    this.login$.pipe(
      switchMap((req) =>
        this.userService.login(req).pipe(
          tap((response) => {
            localStorage.setItem('jwt', response.token);
            void this.router.navigate(['/dashboard']);
          }),
          catchError((err: ApiError) => {
            this.apiErrorHandler.handle(
              err,
              this.fieldServerErrors,
              this.loginError,
              this.errorTranslations,
              this.model()
            );
            return of(null);
          }),
        ),
      ),
    ),
    { initialValue: null },
  );

  onLogin(): void {
    this.loginError.set(null);
    Object.values(this.fieldServerErrors).forEach(s => s.set(null));
    this.login$.next(this.model());
  }
}
