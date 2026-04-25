// core/services/api-error-handler.service.ts
import { Injectable, WritableSignal } from '@angular/core';
import { ApiError } from '../models/users/api-error';

@Injectable({ providedIn: 'root' })
export class ApiErrorHandler {
  translate(error: string, translations: Record<string, string>): string {
    return translations[error] ?? error;
  }

  resolveFieldError(
    field: string,
    fieldSignals: Record<string, WritableSignal<string | null>>,
    formField: () => { touched: () => boolean; valid: () => boolean; errors: () => { message?: string; kind: string }[] },
    translations: Record<string, string>
  ): string | null {
    const serverError = fieldSignals[field]?.();
    if (serverError) return serverError;

    const ctrl = formField();
    if (!ctrl.touched() || ctrl.valid()) return null;

    const errors = ctrl.errors();
    if (errors.length === 0) return null;

    return this.translate(errors[0].message ?? errors[0].kind, translations);
  }

  handle(
    err: ApiError,
    fieldSignals: Record<string, WritableSignal<string | null>>,
    globalError: WritableSignal<string | null>,
    translations: Record<string, string>,
    model: Record<string, unknown>
  ): void {
    if (err.errors && Object.keys(err.errors).length > 0) {
      for (const [key, messages] of Object.entries(err.errors)) {
        const field = key.toLowerCase();
        if (field in model) {
          fieldSignals[field]?.set(this.translate(messages[0], translations));
          continue;
        }
        globalError.set(this.translate(messages[0], translations));
      }
      return;
    }
    globalError.set(this.translate(err.detail || 'Wystąpił błąd', translations));
  }
}
