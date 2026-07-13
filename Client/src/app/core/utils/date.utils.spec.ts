import { todayLocalISODate } from './date.utils';

describe('todayLocalISODate', () => {
  it('returns the current local date in YYYY-MM-DD format', () => {
    expect(todayLocalISODate()).toMatch(/^\d{4}-\d{2}-\d{2}$/);
  });

  it('matches the local date fields, not a UTC-shifted date', () => {
    const d = new Date();
    const expected = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    expect(todayLocalISODate()).toBe(expected);
  });
});
