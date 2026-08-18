import { expenseTotal } from './expense.utils';

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
