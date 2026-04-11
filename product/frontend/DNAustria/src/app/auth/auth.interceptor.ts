import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { environment } from '../environment';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const isApiRequest = request.url.startsWith(environment.apiUrl);
  const nextRequest = isApiRequest ? request.clone({ withCredentials: true }) : request;

  return next(nextRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthEndpoint = nextRequest.url.includes('/api/auth/');

      if (isApiRequest && error.status === 401 && !isAuthEndpoint) {
        authService.setUnauthenticated();
        void router.navigate(['/login'], {
          queryParams: { returnUrl: router.url }
        });
      }

      return throwError(() => error);
    })
  );
};
