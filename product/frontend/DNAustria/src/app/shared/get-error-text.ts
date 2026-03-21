import { HttpErrorResponse } from '@angular/common/http';

export function getErrorText(err: HttpErrorResponse): string | null {
  const body = err.error;
  if (typeof body === 'string' && body.length > 0) {
    return body;
  }
  if (body && typeof body === 'object') {
    if (typeof body.detail === 'string' && body.detail.length > 0) {
      return body.detail;
    }
    if (body.errors && typeof body.errors === 'object') {
      const messages = Object.values(body.errors)
        .flat()
        .filter((m): m is string => typeof m === 'string');
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }
    if (typeof body.title === 'string' && body.title.length > 0) {
      return body.title;
    }
  }
  return null;
}
