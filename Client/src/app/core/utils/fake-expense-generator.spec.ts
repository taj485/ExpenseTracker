import { generateFakeExpenses } from './fake-expense-generator';
import { ALL_CATEGORIES } from './category.utils';
import { todayLocalISODate } from './date.utils';

describe('generateFakeExpenses', () => {
  it('returns between 2 and 4 items, each with a valid category, positive amount, quantity, and today\'s date', () => {
    for (let i = 0; i < 50; i++) {
      const items = generateFakeExpenses();

      expect(items.length).toBeGreaterThanOrEqual(2);
      expect(items.length).toBeLessThanOrEqual(4);

      for (const item of items) {
        expect(ALL_CATEGORIES).toContain(item.category);
        expect(item.amount).toBeGreaterThan(0);
        expect(item.quantity).toBeGreaterThanOrEqual(1);
        expect(item.date).toBe(todayLocalISODate());
      }
    }
  });
});
