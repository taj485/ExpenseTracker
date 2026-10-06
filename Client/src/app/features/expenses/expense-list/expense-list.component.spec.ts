import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { ExpenseListComponent } from './expense-list.component';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { Expense } from '../../../core/models/expense.model';
import { periodRange, todayLocalISODate } from '../../../core/utils/date.utils';

describe('ExpenseListComponent', () => {
  let component: ExpenseListComponent;
  let navigate: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    navigate = vi.fn();

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: { navigate } },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: '7' })), queryParamMap: of(convertToParamMap({})) } },
        { provide: ExpenseService, useValue: { expenses: signal([]), loadAll: vi.fn() } },
        { provide: ExpenseTableService, useValue: { tables: signal([]) } },
      ],
    });

    // Built without rendering: these tests cover state and navigation, not the template.
    component = TestBed.runInInjectionContext(() => new ExpenseListComponent());
    component.tableId.set(7);
  });

  it('starts with every receipt expanded', () => {
    expect(component.isCollapsed('r1')).toBe(false);
  });

  it('collapses a receipt and expands it again', () => {
    component.toggleReceipt('r1');
    expect(component.isCollapsed('r1')).toBe(true);
    expect(component.isCollapsed('r2')).toBe(false);

    component.toggleReceipt('r1');
    expect(component.isCollapsed('r1')).toBe(false);
  });

  it('opens the receipt page from a line on a scanned receipt', () => {
    component.openItem({ receiptId: 42 } as Parameters<ExpenseListComponent['openItem']>[0], 5);
    expect(navigate).toHaveBeenCalledWith(['/expenses/table', 7, 'receipt', 42]);
  });

  it('opens the expense page for a standalone expense', () => {
    component.openItem({ receiptId: null } as Parameters<ExpenseListComponent['openItem']>[0], 5);
    expect(navigate).toHaveBeenCalledWith(['/expenses/table', 7, 5]);
  });

  it('opens on this week when no date filter is set', () => {
    expect(component.selectedPeriod()).toBe('this-week');
    expect(component.totalLabel()).toBe('This Week');
  });

  it('picks a period and clears any month', () => {
    component.selectPeriod('last-week');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({
      queryParams: { period: 'last-week', month: null },
      queryParamsHandling: 'merge',
    }));
  });

  it('shows all months when the selected period is picked again', () => {
    component.selectPeriod('this-week');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { period: 'all', month: null } }));
  });

  it('drops the period when a month is picked', () => {
    component.toggleMonthFilter('2026-09');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-09', period: null } }));
  });

  it('stores All months explicitly, so it does not fall back to this week', () => {
    component.clearMonths();
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: null, period: 'all' } }));
  });

  it('goes back to the default (this week) on reset', () => {
    component.resetFilters();
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: null, period: null, category: null } }));
  });
});

function createWithQuery(query: Record<string, string>, expenses: Expense[], navigate = vi.fn()): ExpenseListComponent {
  TestBed.configureTestingModule({
    providers: [
      { provide: Router, useValue: { navigate } },
      { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: '7' })), queryParamMap: of(convertToParamMap(query)) } },
      { provide: ExpenseService, useValue: { expenses: signal(expenses), loadAll: vi.fn() } },
      { provide: ExpenseTableService, useValue: { tables: signal([]) } },
    ],
  });
  return TestBed.runInInjectionContext(() => new ExpenseListComponent());
}

describe('ExpenseListComponent date periods', () => {
  const expenseOn = (id: number, day: string, unitPrice: number) =>
    ({ id, date: `${day}T00:00:00`, unitPrice, quantity: 1, category: 'Food' }) as Expense;

  const thisWeek = periodRange('this-week');
  const lastWeek = periodRange('last-week');
  const expenses = [
    expenseOn(1, todayLocalISODate(), 2),
    expenseOn(2, thisWeek.start, 3),
    expenseOn(3, lastWeek.end, 5),
    expenseOn(4, '2020-01-01', 7),
  ];

  it('shows only this week by default', () => {
    const ids = createWithQuery({}, expenses).filteredExpenses().map(e => e.id);
    expect(ids).toContain(1);
    expect(ids).toContain(2);
    expect(ids).not.toContain(3);
    expect(ids).not.toContain(4);
  });

  it('shows only last week', () => {
    expect(createWithQuery({ period: 'last-week' }, expenses).filteredExpenses().map(e => e.id)).toEqual([3]);
  });

  it('shows only today', () => {
    const component = createWithQuery({ period: 'today' }, expenses);
    expect(component.filteredExpenses().map(e => e.id)).toContain(1);
    expect(component.filteredExpenses().map(e => e.id)).not.toContain(3);
    expect(component.totalLabel()).toBe('Today');
  });

  it('shows everything for period=all', () => {
    const component = createWithQuery({ period: 'all' }, expenses);
    expect(component.selectedPeriod()).toBeNull();
    expect(component.filteredCount()).toBe(4);
    expect(component.totalLabel()).toBe('Total');
  });

  it('lets a month in the URL win over a period', () => {
    const component = createWithQuery({ period: 'today', month: '2020-01' }, expenses);
    expect(component.selectedPeriod()).toBeNull();
    expect(component.filteredExpenses().map(e => e.id)).toEqual([4]);
  });
});

describe('ExpenseListComponent with several months selected', () => {
  let component: ExpenseListComponent;
  let navigate: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    navigate = vi.fn();
    const expenses = [
      { id: 1, date: '2026-10-02T00:00:00', unitPrice: 1, quantity: 1, category: 'Food' },
      { id: 2, date: '2026-09-15T00:00:00', unitPrice: 2, quantity: 1, category: 'Food' },
      { id: 3, date: '2026-08-20T00:00:00', unitPrice: 4, quantity: 1, category: 'Food' },
    ] as Expense[];

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: { navigate } },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: '7' })), queryParamMap: of(convertToParamMap({ month: '2026-08,2026-10' })) } },
        { provide: ExpenseService, useValue: { expenses: signal(expenses), loadAll: vi.fn() } },
        { provide: ExpenseTableService, useValue: { tables: signal([]) } },
      ],
    });
    component = TestBed.runInInjectionContext(() => new ExpenseListComponent());
  });

  it('shows expenses from every selected month', () => {
    expect(component.selectedMonths()).toEqual(['2026-10', '2026-08']);
    expect(component.filteredExpenses().map(e => e.id)).toEqual([1, 3]);
    expect(component.filteredTotal()).toBe(5);
    expect(component.totalLabel()).toBe('2 Months');
  });

  it('adds a month to the selection', () => {
    component.toggleMonthFilter('2026-09');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-10,2026-09,2026-08', period: null } }));
  });

  it('removes a selected month', () => {
    component.toggleMonthFilter('2026-10');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-08', period: null } }));
  });
});
