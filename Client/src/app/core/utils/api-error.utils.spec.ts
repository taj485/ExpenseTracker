import { HttpErrorResponse } from '@angular/common/http';
import { apiErrorMessage } from './api-error.utils';

const httpError = (error: unknown) => new HttpErrorResponse({ status: 400, error });

describe('apiErrorMessage', () => {
  it('returns the message from an { error } body', () => {
    expect(apiErrorMessage(httpError({ error: 'Only an admin can delete this expense table' }), 'fallback'))
      .toBe('Only an admin can delete this expense table');
  });

  it('returns the first validation message from an { errors } body', () => {
    const body = { title: 'Validation failed', errors: [{ field: 'InviteeEmail', message: 'Invitee email is not a valid email address.' }] };
    expect(apiErrorMessage(httpError(body), 'fallback')).toBe('Invitee email is not a valid email address.');
  });

  it('skips validation entries without a message', () => {
    const body = { errors: [{ field: 'Name' }, { field: 'Name', message: 'Name is required.' }] };
    expect(apiErrorMessage(httpError(body), 'fallback')).toBe('Name is required.');
  });

  it('falls back when the body has no message', () => {
    expect(apiErrorMessage(httpError({ title: 'Internal server error' }), 'fallback')).toBe('fallback');
    expect(apiErrorMessage(httpError(null), 'fallback')).toBe('fallback');
    expect(apiErrorMessage(httpError({ error: '   ' }), 'fallback')).toBe('fallback');
  });

  it('falls back for errors that are not HTTP responses', () => {
    expect(apiErrorMessage(new Error('boom'), 'fallback')).toBe('fallback');
  });
});
