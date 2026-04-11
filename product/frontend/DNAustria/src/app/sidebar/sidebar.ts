import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-sidebar',
  imports: [
    RouterModule
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class Sidebar {
  protected readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected logoutInProgress = false;

  protected logout(): void {
    if (this.logoutInProgress) {
      return;
    }

    this.logoutInProgress = true;
    this.authService.logout()
      .pipe(finalize(() => this.logoutInProgress = false))
      .subscribe({
        next: () => {
          void this.router.navigate(['/login']);
        },
        error: () => {
          void this.router.navigate(['/login']);
        }
      });
  }
}
