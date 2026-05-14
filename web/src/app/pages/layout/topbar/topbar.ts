import { Component, inject, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { NgIcon } from '@ng-icons/core';
import { catchError, of } from 'rxjs';
import { UserService } from '../../../core/services/user/user';

@Component({
  selector: 'app-topbar',
  imports: [RouterLink, NgIcon],
  templateUrl: './topbar.html',
})
export class Topbar {
  private readonly userService = inject(UserService);

  readonly menuToggle = output<void>();
  readonly logout     = output<void>();
  readonly userMenuOpen = signal(false);

  readonly user = toSignal(
    this.userService.getMe().pipe(catchError(() => of(null)))
  );

  toggleUserMenu(): void { this.userMenuOpen.update(v => !v); }
}
