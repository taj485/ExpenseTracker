import { uploaderLabel } from './uploader.utils';

describe('uploaderLabel', () => {
  it('says "you" for the current user, whatever the email', () => {
    const mine = { createdByEmail: 'me@example.com', createdByCurrentUser: true };
    expect(uploaderLabel(mine, 'short')).toBe('you');
    expect(uploaderLabel(mine, 'full')).toBe('You');
  });

  it('uses the part before @ for short labels and the whole email for full ones', () => {
    const theirs = { createdByEmail: 'emma.carter@example.com', createdByCurrentUser: false };
    expect(uploaderLabel(theirs, 'short')).toBe('emma.carter');
    expect(uploaderLabel(theirs, 'full')).toBe('emma.carter@example.com');
  });

  it('returns null when the uploader was not recorded', () => {
    expect(uploaderLabel({ createdByEmail: null, createdByCurrentUser: false }, 'short')).toBeNull();
    expect(uploaderLabel({ createdByEmail: '  ', createdByCurrentUser: false }, 'full')).toBeNull();
  });

  it('copes with an older API response that omits the fields', () => {
    expect(uploaderLabel({}, 'short')).toBeNull();
  });
});
