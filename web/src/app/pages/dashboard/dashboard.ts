import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { ApiError } from '../../core/models/users/api-error';
import { UserDetailsResponse } from '../../core/models/users/user-details-response';
import { UserService } from '../../core/services/user/user';

@Component({
  selector: 'app-dashboard',
  imports: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="min-h-screen bg-brand-dark text-white px-6 py-10">
      <div class="mx-auto max-w-2xl space-y-6">
        <h1 class="text-2xl font-bold tracking-tight">Dashboard</h1>

        <button
          type="button"
          class="rounded bg-blue-600 px-4 py-2 font-medium hover:bg-blue-500 disabled:opacity-60"
          [disabled]="isLoading()"
          (click)="onTestGetMe()"
        >
          {{ isLoading() ? 'Sprawdzam...' : 'Test getMe' }}
        </button>

        @if (errorMessage()) {
          <p class="rounded border border-red-500 bg-red-500/10 p-3 text-red-200">
            {{ errorMessage() }}
          </p>
        }

        @if (currentUser()) {
          <section class="rounded border border-white/20 p-4 space-y-2">
            <h2 class="text-lg font-semibold">Dane z endpointu /user/me</h2>
            <p><strong>ID:</strong> {{ currentUser()!.id }}</p>
            <p><strong>Email:</strong> {{ currentUser()!.email }}</p>
            <p><strong>Imię i nazwisko:</strong> {{ currentUser()!.firstName }} {{ currentUser()!.lastName }}</p>
            <p><strong>Uprawnienia:</strong> {{ currentUser()!.permissions.length }}</p>
          </section>
        }
      </div>
    </div>
  `,
})
export class Dashboard {
  private readonly userService = inject(UserService);

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly currentUser = signal<UserDetailsResponse | null>(null);

  onTestGetMe(): void {
    this.errorMessage.set(null);
    this.isLoading.set(true);

    this.userService.getMe().pipe(
      finalize(() => this.isLoading.set(false)),
    ).subscribe({
      next: (user) => this.currentUser.set(user),
      error: (error: ApiError) => this.errorMessage.set(error.detail ?? 'Nie udało się pobrać danych użytkownika'),
    });
  }
}
