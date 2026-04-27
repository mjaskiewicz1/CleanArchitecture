import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, Observable, switchMap, throwError, shareReplay, finalize } from 'rxjs';
import { AuthSessionService } from '../services/auth/auth-session';
import { UserService } from '../services/user/user';
import { LoginResponse } from '../models/users/login-response';

let refreshRequest$: Observable<LoginResponse> | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authSession = inject(AuthSessionService);
  const userService = inject(UserService);
  const router = inject(Router);

  const accessToken = authSession.accessToken();
  const shouldSkipAuth = isAuthFreeEndpoint(req.url);

  const authorizedRequest = !shouldSkipAuth && accessToken
    ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
    : req;

  return next(authorizedRequest).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse) || error.status !== 401) {
        return throwError(() => error);
      }

      if (shouldSkipAuth || isRefreshEndpoint(req.url)) {
        authSession.clearSession();
        void router.navigate(['/login']);
        return throwError(() => error);
      }

      const refreshToken = authSession.refreshToken();
      if (!refreshToken) {
        authSession.clearSession();
        void router.navigate(['/login']);
        return throwError(() => error);
      }

      const refresh$ = refreshRequest$ ?? userService.refreshToken({ refreshToken }).pipe(
        shareReplay(1),
        finalize(() => {
          refreshRequest$ = null;
        }),
      );
      refreshRequest$ = refresh$;

      return refresh$.pipe(
        switchMap((tokens) => {
          authSession.setSession(tokens);
          const retriedRequest = req.clone({
            setHeaders: { Authorization: `Bearer ${tokens.accessToken}` },
          });
          return next(retriedRequest);
        }),
        catchError((refreshError) => {
          authSession.clearSession();
          void router.navigate(['/login']);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};

function isAuthFreeEndpoint(url: string): boolean {
  return isLoginEndpoint(url) || isRefreshEndpoint(url);
}

function isLoginEndpoint(url: string): boolean {
  return url.includes('/api/user/login');
}

function isRefreshEndpoint(url: string): boolean {
  return url.includes('/api/user/refresh-token');
}
