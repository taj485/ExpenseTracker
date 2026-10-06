/** A selectable month: 'YYYY-MM' plus the label shown in the dropdown. */
export interface MonthOption {
  key: string;
  label: string;
}

export function todayLocalISODate(): string {
  return localISODate(new Date());
}

function localISODate(d: Date): string {
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

/** A rolling date filter on the expense list. Weeks run Monday to Sunday. */
export type DatePeriod = 'this-week' | 'last-week' | 'today';

export const DATE_PERIODS: readonly DatePeriod[] = ['today', 'this-week', 'last-week'];

/** First and last local day ('YYYY-MM-DD', inclusive) of a period, relative to `now`. */
export function periodRange(period: DatePeriod, now = new Date()): { start: string; end: string } {
  if (period === 'today') {
    const today = localISODate(now);
    return { start: today, end: today };
  }
  // getDay() is 0 for Sunday, so shift it to make Monday day 0.
  const daysSinceMonday = (now.getDay() + 6) % 7;
  const weeksBack = period === 'last-week' ? 1 : 0;
  // Building from y/m/d lets Date roll over month and year ends, and dodges DST hour shifts.
  const monday = new Date(now.getFullYear(), now.getMonth(), now.getDate() - daysSinceMonday - 7 * weeksBack);
  const sunday = new Date(monday.getFullYear(), monday.getMonth(), monday.getDate() + 6);
  return { start: localISODate(monday), end: localISODate(sunday) };
}

// Local date fields throughout, matching todayLocalISODate — an expense dated the 1st at
// 00:30 UTC belongs to the month the user saw on the form, not the previous one.
function monthKey(date: Date): string {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
}

/** Current month as 'YYYY-MM'. */
export function currentMonthKey(): string {
  return monthKey(new Date());
}

/** The 'YYYY-MM' an expense date falls in. */
export function monthKeyOf(isoDate: string): string {
  return monthKey(new Date(isoDate));
}

/**
 * The current month plus the given number of preceding months, newest first.
 * monthsBack of 12 therefore yields 13 keys — far enough back to reach a year ago.
 */
export function monthKeysBack(monthsBack: number): string[] {
  const now = new Date();
  const keys: string[] = [];

  for (let i = 0; i <= monthsBack; i++) {
    // Day 1 avoids the month-end rollover trap: setting month on the 31st can skip a month.
    keys.push(monthKey(new Date(now.getFullYear(), now.getMonth() - i, 1)));
  }

  return keys;
}

/** '2026-08' becomes 'August 2026'. */
export function formatMonthKey(key: string): string {
  const [year, month] = key.split('-').map(Number);
  return new Date(year, month - 1, 1)
    .toLocaleDateString('en-GB', { month: 'long', year: 'numeric' });
}
