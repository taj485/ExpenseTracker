import { HttpErrorResponse } from '@angular/common/http';

/**
 * Message from the API's error body (ExceptionHandlingMiddleware): domain / forbidden / not-found
 * errors send `{ error }`, validation failures send `{ errors: [{ field, message }] }`.
 * Falls back when neither carries a message.
 */
export function apiErrorMessage(err: unknown, fallback: string): string {
  const body = err instanceof HttpErrorResponse ? err.error : null;

  if (typeof body?.error === 'string' && body.error.trim()) return body.error;

  const firstValidation = Array.isArray(body?.errors)
    ? body.errors.find((e: { message?: unknown }) => typeof e?.message === 'string' && e.message.trim())
    : null;

  return firstValidation?.message ?? fallback;
}
