import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { BACKEND_URL, API_KEY } from './api.tokens';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const apiInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const base = inject(BACKEND_URL);
  const key = inject(API_KEY);
  let url = req.url;
  if (!/^https?:\/\//.test(url)) {
    url = base.replace(/\/$/, '') + '/' + url.replace(/^\//, '');
  }
  const authReq = req.clone({
    url,
    setHeaders: { 'X-API-Key': key }
  });
  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      console.error('API error', err.status, err.message);
      return throwError(() => err);
    })
  );
};
export const environment = {
  production: false,
  backendUrl: 'http://localhost:5100',
  apiKey: 'dev-key'
};

