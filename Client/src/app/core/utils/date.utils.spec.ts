import {
  currentMonthKey,
  formatMonthKey,
  monthKeyOf,
  monthKeysBack,
  todayLocalISODate,
} from './date.utils';

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

describe('currentMonthKey', () => {
  it('returns the current local month as YYYY-MM', () => {
    const d = new Date();
    const expected = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`;
    expect(currentMonthKey()).toBe(expected);
  });
});

describe('monthKeyOf', () => {
  it('extracts the month from an ISO date', () => {
    expect(monthKeyOf('2026-08-19T00:00:00Z')).toMatch(/^2026-0[78]$/);
  });

  it('zero-pads single-digit months', () => {
    expect(monthKeyOf(new Date(2026, 2, 15).toISOString())).toBe('2026-03');
  });

  it('groups every day of a month under the same key', () => {
    const first = monthKeyOf(new Date(2026, 5, 1, 12).toISOString());
    const last = monthKeyOf(new Date(2026, 5, 30, 12).toISOString());
    expect(first).toBe(last);
    expect(first).toBe('2026-06');
  });
});

describe('monthKeysBack', () => {
  it('returns the current month plus the requested number of earlier months', () => {
    expect(monthKeysBack(12)).toHaveLength(13);
    expect(monthKeysBack(0)).toEqual([currentMonthKey()]);
  });

  it('starts at the current month and runs backwards', () => {
    const keys = monthKeysBack(12);
    expect(keys[0]).toBe(currentMonthKey());
    expect([...keys].sort().reverse()).toEqual(keys);
  });

  it('reaches exactly one year back', () => {
    const keys = monthKeysBack(12);
    const now = new Date();
    const yearAgo = new Date(now.getFullYear(), now.getMonth() - 12, 1);
    const expected = `${yearAgo.getFullYear()}-${String(yearAgo.getMonth() + 1).padStart(2, '0')}`;
    expect(keys[keys.length - 1]).toBe(expected);
  });

  it('rolls back over a year boundary without skipping a month', () => {
    const keys = monthKeysBack(12);
    expect(new Set(keys).size).toBe(keys.length);
  });
});

describe('formatMonthKey', () => {
  it('renders a readable month and year', () => {
    expect(formatMonthKey('2026-08')).toBe('August 2026');
    expect(formatMonthKey('2026-01')).toBe('January 2026');
    expect(formatMonthKey('2025-12')).toBe('December 2025');
  });
});
