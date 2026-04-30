import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiError } from '../models/users/api-error';

export const errorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const apiError: ApiError = isApiError(error.error)
        ? error.error
        : { title: 'Błąd połączenia', detail: 'Nie można połączyć się z serwerem', status: 0, errors: {} } as ApiError;
      return throwError(() => apiError);
    })
  );

function isApiError(value: unknown): value is ApiError {
  return typeof value === 'object' && value !== null && 'detail' in value;
}
