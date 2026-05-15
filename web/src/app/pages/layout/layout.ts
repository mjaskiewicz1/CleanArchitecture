import { Component, signal, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { NgTemplateOutlet } from '@angular/common';
import { NgIcon } from '@ng-icons/core';
import { AuthSessionService } from '../../core/services/auth/auth-session';
import { UserService } from '../../core/services/user/user';
import { Topbar } from './topbar/topbar';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgTemplateOutlet, Topbar, NgIcon],
  templateUrl: './layout.html',
})
export class Layout {
  private readonly auth = inject(AuthSessionService);
  private readonly router = inject(Router);
  private readonly userService = inject(UserService);

  readonly menuOpen = signal(false);

  toggleMenu(): void {
    this.menuOpen.update((v) => !v);
  }

  logout(): void {
    const clear = () => {
      this.auth.clearSession();
      void this.router.navigate(['/login']);
    };
    this.userService.revokeToken().subscribe({ complete: clear, error: clear });
  }
}
