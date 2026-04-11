import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, catchError, firstValueFrom, map, of, tap } from 'rxjs';
import { environment } from '../environment';

type AuthStatus = 'unknown' | 'authenticated' | 'unauthenticated';

export interface AuthUser {
  username: string;
  displayName: string;
  email?: string | null;
}

interface LoginResponse {
  user: AuthUser;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly authStatus = signal<AuthStatus>('unknown');
  private readonly currentUser = signal<AuthUser | null>(null);
  private initRequest?: Promise<boolean>;

  readonly isAuthenticated = computed(() => this.authStatus() === 'authenticated');
  readonly username = computed(() => this.currentUser()?.username ?? '');
  readonly displayName = computed(() => this.currentUser()?.displayName ?? '');
  readonly user = this.currentUser.asReadonly();

  ensureInitialized(): Promise<boolean> {
    if (!this.initRequest) {
      this.initRequest = firstValueFrom(
        this.http.get<AuthUser>(`${environment.apiUrl}/api/auth/me`).pipe(
          tap(user => this.setAuthenticated(user)),
          map(() => true),
          catchError(() => {
            this.setUnauthenticated();
            return of(false);
          })
        )
      );
    }

    return this.initRequest;
  }

  login(username: string, password: string): Observable<AuthUser> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, { username, password }).pipe(
      map(response => response.user),
      tap(user => {
        this.setAuthenticated(user);
        this.initRequest = Promise.resolve(true);
      })
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/auth/logout`, {}).pipe(
      tap(() => this.setUnauthenticated()),
      catchError(error => {
        this.setUnauthenticated();
        return of(void 0);
      })
    );
  }

  setUnauthenticated(): void {
    this.currentUser.set(null);
    this.authStatus.set('unauthenticated');
    this.initRequest = Promise.resolve(false);
  }

  private setAuthenticated(user: AuthUser): void {
    this.currentUser.set(user);
    this.authStatus.set('authenticated');
  }
}
