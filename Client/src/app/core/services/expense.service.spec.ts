import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { ExpenseService } from './expense.service';
import { Expense } from '../models/expense.model';
import { currentMonthKey } from '../utils/date.utils';

/** Local-time date in a month offset from the current one, mid-month to dodge boundaries. */
function dateInMonth(monthsAgo: number): string {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth() - monthsAgo, 15, 12).toISOString();
}

function monthKeyAgo(monthsAgo: number): string {
  const now = new Date();
  const d = new Date(now.getFullYear(), now.getMonth() - monthsAgo, 1);
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`;
}

let nextId = 1;
function expense(partial: Partial<Expense> & { date: string }): Expense {
  return {
    id: nextId++,
    unitPrice: 10,
    quantity: 1,
    currency: 'GBP',
    description: 'Test',
    category: 'Food',
    merchantId: null,
    merchant: null,
    merchantWebsite: null,
    receiptId: null,
    ...partial,
  };
}

describe('ExpenseService month selection', () => {
  let service: ExpenseService;

  beforeEach(() => {
    nextId = 1;
    TestBed.configureTestingModule({ providers: [provideHttpClient()] });
    service = TestBed.inject(ExpenseService);
  });

  it('defaults to the current month', () => {
    expect(service.selectedMonth()).toBe(currentMonthKey());
  });

  it('offers the current month plus twelve back, with no expenses loaded at all', () => {
    const keys = service.availableMonths().map(m => m.key);

    expect(keys).toHaveLength(13);
    expect(keys[0]).toBe(currentMonthKey());
    expect(keys[12]).toBe(monthKeyAgo(12));
  });

  it('labels each month readably', () => {
    expect(service.availableMonths()[0].label).toMatch(/^[A-Z][a-z]+ \d{4}$/);
  });

  it('lists months newest first', () => {
    const keys = service.availableMonths().map(m => m.key);
    expect([...keys].sort().reverse()).toEqual(keys);
  });

  it('keeps a month older than the window when it holds expenses', () => {
    service.expenses.set([expense({ date: dateInMonth(20) })]);

    const keys = service.availableMonths().map(m => m.key);

    expect(keys).toContain(monthKeyAgo(20));
    expect(keys).toHaveLength(14);
  });

  it('does not duplicate a month that both holds expenses and is in the window', () => {
    service.expenses.set([
      expense({ date: dateInMonth(2) }),
      expense({ date: dateInMonth(2) }),
    ]);

    const keys = service.availableMonths().map(m => m.key);

    expect(keys).toHaveLength(13);
    expect(new Set(keys).size).toBe(13);
  });

  it('totals only the selected month', () => {
    service.expenses.set([
      expense({ date: dateInMonth(0), unitPrice: 10, quantity: 2 }),  // 20 this month
      expense({ date: dateInMonth(1), unitPrice: 5, quantity: 1 }),   // 5 last month
    ]);

    expect(service.selectedMonthSpent()).toBe(20);
    expect(service.transactionCount()).toBe(1);

    service.selectedMonth.set(monthKeyAgo(1));

    expect(service.selectedMonthSpent()).toBe(5);
    expect(service.transactionCount()).toBe(1);
  });

  it('recomputes the category breakdown for the selected month', () => {
    service.expenses.set([
      expense({ date: dateInMonth(0), category: 'Food', unitPrice: 30 }),
      expense({ date: dateInMonth(3), category: 'Transport', unitPrice: 40 }),
    ]);

    expect(service.categoryBreakdown().map(s => s.category)).toEqual(['Food']);

    service.selectedMonth.set(monthKeyAgo(3));

    const breakdown = service.categoryBreakdown();
    expect(breakdown).toHaveLength(1);
    expect(breakdown[0].category).toBe('Transport');
    expect(breakdown[0].percentage).toBe(100);
  });

  it('reports the top category for the selected month', () => {
    service.expenses.set([
      expense({ date: dateInMonth(1), category: 'Food', unitPrice: 5 }),
      expense({ date: dateInMonth(1), category: 'Health', unitPrice: 50 }),
      expense({ date: dateInMonth(0), category: 'Food', unitPrice: 100 }),
    ]);

    expect(service.topCategory()).toEqual({ name: 'Food', total: 100 });

    service.selectedMonth.set(monthKeyAgo(1));

    expect(service.topCategory()).toEqual({ name: 'Health', total: 50 });
  });

  it('shows an empty month as zero rather than falling back to another month', () => {
    service.expenses.set([expense({ date: dateInMonth(0), unitPrice: 99 })]);

    service.selectedMonth.set(monthKeyAgo(6));

    expect(service.selectedMonthSpent()).toBe(0);
    expect(service.transactionCount()).toBe(0);
    expect(service.categoryBreakdown()).toEqual([]);
    expect(service.topCategory()).toBeNull();
  });

  it('keeps currentMonthSpent on the calendar month when another month is selected', () => {
    service.expenses.set([
      expense({ date: dateInMonth(0), unitPrice: 100 }),
      expense({ date: dateInMonth(4), unitPrice: 7 }),
    ]);

    service.selectedMonth.set(monthKeyAgo(4));

    // The dashboard follows the dropdown; the expense list's "This Month" card must not.
    expect(service.selectedMonthSpent()).toBe(7);
    expect(service.currentMonthSpent()).toBe(100);
  });

  it('still offers a selected month that holds nothing', () => {
    service.selectedMonth.set(monthKeyAgo(6));

    expect(service.availableMonths().map(m => m.key)).toContain(monthKeyAgo(6));
  });

  describe('merchantBreakdown', () => {
    it('is empty when the month has no expenses', () => {
      expect(service.merchantBreakdown()).toEqual([]);
    });

    it('groups by merchantId, not by spelling', () => {
      service.expenses.set([
        expense({ date: dateInMonth(0), merchantId: 3, merchant: 'Asda', unitPrice: 10 }),
        expense({ date: dateInMonth(0), merchantId: 3, merchant: 'ASDA', unitPrice: 15 }),
      ]);

      const stats = service.merchantBreakdown();

      expect(stats).toHaveLength(1);
      expect(stats[0].total).toBe(25);
      expect(stats[0].count).toBe(2);
      expect(stats[0].percentage).toBe(100);
    });

    it('ranks merchants by spend, highest first', () => {
      service.expenses.set([
        expense({ date: dateInMonth(0), merchantId: 1, merchant: 'Tesco', unitPrice: 10 }),
        expense({ date: dateInMonth(0), merchantId: 2, merchant: 'Shell', unitPrice: 40 }),
        expense({ date: dateInMonth(0), merchantId: 3, merchant: 'Asda', unitPrice: 25 }),
      ]);

      expect(service.merchantBreakdown().map(s => s.merchant)).toEqual(['Shell', 'Asda', 'Tesco']);
    });

    it('labels expenses with no merchant as Unknown', () => {
      service.expenses.set([expense({ date: dateInMonth(0), merchantId: null, merchant: null })]);

      expect(service.merchantBreakdown()[0].merchant).toBe('Unknown');
    });

    it('keeps the website so the legend can show a real logo', () => {
      service.expenses.set([
        expense({ date: dateInMonth(0), merchantId: 1, merchant: 'Tesco', merchantWebsite: 'tesco.com' }),
      ]);

      expect(service.merchantBreakdown()[0].website).toBe('tesco.com');
    });

    it('folds everything past the top five into a single Other slice', () => {
      service.expenses.set([100, 90, 80, 70, 60, 50, 40].map((price, i) =>
        expense({ date: dateInMonth(0), merchantId: i + 1, merchant: `Shop ${i + 1}`, unitPrice: price }),
      ));

      const stats = service.merchantBreakdown();

      expect(stats).toHaveLength(6);
      expect(stats[5].isOther).toBe(true);
      expect(stats[5].merchant).toBe('Other (2)');
      expect(stats[5].total).toBe(90);   // 50 + 40
      expect(stats[5].count).toBe(2);
      expect(stats.filter(s => s.isOther)).toHaveLength(1);
    });

    it('does not add an Other slice at exactly five merchants', () => {
      service.expenses.set([5, 4, 3, 2, 1].map((price, i) =>
        expense({ date: dateInMonth(0), merchantId: i + 1, merchant: `Shop ${i + 1}`, unitPrice: price }),
      ));

      const stats = service.merchantBreakdown();

      expect(stats).toHaveLength(5);
      expect(stats.some(s => s.isOther)).toBe(false);
    });

    it('only counts the selected month', () => {
      service.expenses.set([
        expense({ date: dateInMonth(0), merchantId: 1, merchant: 'Tesco', unitPrice: 10 }),
        expense({ date: dateInMonth(2), merchantId: 2, merchant: 'Shell', unitPrice: 99 }),
      ]);

      expect(service.merchantBreakdown().map(s => s.merchant)).toEqual(['Tesco']);

      service.selectedMonth.set(monthKeyAgo(2));

      expect(service.merchantBreakdown().map(s => s.merchant)).toEqual(['Shell']);
    });

    it('accounts for quantity in the totals', () => {
      service.expenses.set([
        expense({ date: dateInMonth(0), merchantId: 1, merchant: 'Tesco', unitPrice: 10, quantity: 3 }),
      ]);

      expect(service.merchantBreakdown()[0].total).toBe(30);
    });
  });
});
