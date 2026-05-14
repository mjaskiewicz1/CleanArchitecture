import { Component, signal, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { NgTemplateOutlet } from '@angular/common';
import { AuthSessionService } from '../../core/services/auth/auth-session';
import { Topbar } from './topbar/topbar';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgTemplateOutlet, Topbar],
  templateUrl: './layout.html',
})
export class Layout {
  private readonly auth   = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly menuOpen = signal(false);

  toggleMenu(): void { this.menuOpen.update(v => !v); }

  logout(): void {
    this.auth.clearSession();
    void this.router.navigate(['/login']);
  }
}
