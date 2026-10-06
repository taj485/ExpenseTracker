import { dayKeyOf, todayLocalISODate } from './date.utils';

/** Line total for an expense: unit price x quantity, rounded to whole pence. */
export function expenseTotal(expense: { unitPrice: number; quantity: number }): number {
  return Math.round(expense.unitPrice * expense.quantity * 100) / 100;
}

/** One day's spend in a month, for the daily bar chart. */
export interface DailySpend {
  /** 'YYYY-MM-DD' */
  date: string;
  /** Day of the month, 1-31. */
  day: number;
  total: number;
  /** Still to come, so it is left empty rather than shown as £0. */
  isFuture: boolean;
}

/** Spend per day for every day of a 'YYYY-MM' month, in order. */
export function dailyTotals(
  expenses: { date: string; unitPrice: number; quantity: number }[],
  monthKey: string,
  today = todayLocalISODate(),
): DailySpend[] {
  const totals = new Map<string, number>();
  for (const e of expenses) {
    const day = dayKeyOf(e.date);
    if (day.startsWith(monthKey)) totals.set(day, (totals.get(day) ?? 0) + expenseTotal(e));
  }

  const [year, month] = monthKey.split('-').map(Number);
  // Day 0 of the next month is the last day of this one.
  const daysInMonth = new Date(year, month, 0).getDate();

  return Array.from({ length: daysInMonth }, (_, i) => {
    const date = `${monthKey}-${String(i + 1).padStart(2, '0')}`;
    // Summing pence-rounded lines can still drift (0.1 + 0.2), so round the day too.
    const total = Math.round((totals.get(date) ?? 0) * 100) / 100;
    return { date, day: i + 1, total, isFuture: date > today };
  });
}
