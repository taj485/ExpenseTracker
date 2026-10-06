import { dailyTotals, expenseTotal } from './expense.utils';

describe('expenseTotal', () => {
  it('returns the unit price unchanged for a single unit', () => {
    expect(expenseTotal({ unitPrice: 12.5, quantity: 1 })).toBe(12.5);
  });

  it('multiplies the unit price by the quantity', () => {
    expect(expenseTotal({ unitPrice: 2.5, quantity: 3 })).toBe(7.5);
  });

  it('rounds floating point drift to whole pence', () => {
    // 0.1 * 3 is 0.30000000000000004 in binary floating point.
    expect(expenseTotal({ unitPrice: 0.1, quantity: 3 })).toBe(0.3);
  });

  it('rounds a sub-penny product up to the nearest penny', () => {
    // 3.333 * 3 = 9.999
    expect(expenseTotal({ unitPrice: 3.333, quantity: 3 })).toBe(10);
  });

  it('rounds an exact half-penny down when binary floating point falls short', () => {
    // 1.005 * 100 is 100.49999999999999, so Math.round lands on 100.
    // Documented rather than corrected: this matches the rounding the
    // receipt upload flow already used before units were persisted.
    expect(expenseTotal({ unitPrice: 1.005, quantity: 1 })).toBe(1);
  });

  it('returns zero when the quantity is zero', () => {
    expect(expenseTotal({ unitPrice: 9.99, quantity: 0 })).toBe(0);
  });
});

describe('dailyTotals', () => {
  const line = (date: string, unitPrice: number, quantity = 1) => ({ date: `${date}T00:00:00`, unitPrice, quantity });

  it('returns every day of the month, summing each day', () => {
    const days = dailyTotals([line('2026-09-03', 2), line('2026-09-03', 1.5, 2), line('2026-09-30', 4)], '2026-09', '2026-10-06');
    expect(days.length).toBe(30);
    expect(days[0]).toEqual({ date: '2026-09-01', day: 1, total: 0, isFuture: false });
    expect(days[2].total).toBe(5);
    expect(days[29].total).toBe(4);
  });

  it('ignores other months', () => {
    const days = dailyTotals([line('2026-08-31', 9), line('2026-10-01', 9)], '2026-09', '2026-10-06');
    expect(days.every(d => d.total === 0)).toBe(true);
  });

  it('marks the days still to come in the current month', () => {
    const days = dailyTotals([], '2026-10', '2026-10-06');
    expect(days.length).toBe(31);
    expect(days[5].isFuture).toBe(false);
    expect(days[6].isFuture).toBe(true);
  });

  it('handles a leap-year February', () => {
    expect(dailyTotals([], '2028-02', '2028-03-01').length).toBe(29);
  });

  it('rounds each day to whole pence', () => {
    expect(dailyTotals([line('2026-09-01', 0.1), line('2026-09-01', 0.2)], '2026-09', '2026-10-06')[0].total).toBe(0.3);
  });
});
