import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { ExpenseListComponent } from './expense-list.component';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { Expense } from '../../../core/models/expense.model';

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

  it('opens on all months when no date filter is set', () => {
    expect(component.selectedMonths()).toEqual([]);
    expect(component.totalLabel()).toBe('Total');
  });

  it('picks a month', () => {
    component.toggleMonthFilter('2026-09');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-09' }, queryParamsHandling: 'merge' }));
  });

  it('clears the months for All months', () => {
    component.clearMonths();
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: null } }));
  });

  it('clears months and categories on reset', () => {
    component.resetFilters();
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: null, category: null } }));
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
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-10,2026-09,2026-08' } }));
  });

  it('removes a selected month', () => {
    component.toggleMonthFilter('2026-10');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-08' } }));
  });
});

describe('ExpenseListComponent filters and month list', () => {
  let navigate: ReturnType<typeof vi.fn>;

  function build(query: Record<string, string>): ExpenseListComponent {
    navigate = vi.fn();
    const expenses = [
      { id: 1, date: '2026-10-02T00:00:00', unitPrice: 1, quantity: 1, category: 'Food' },
      { id: 2, date: '2026-09-15T00:00:00', unitPrice: 2, quantity: 1, category: 'Health' },
    ] as Expense[];

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

  afterEach(() => vi.useRealTimers());

  it('shows every expense when no month is selected', () => {
    const component = build({});
    expect(component.filteredExpenses().map(e => e.id)).toEqual([1, 2]);
  });

  it('shows only the selected category', () => {
    const component = build({ category: 'Health' });
    expect(component.filteredExpenses().map(e => e.id)).toEqual([2]);
  });

  it('labels a single selected month as This Month', () => {
    expect(build({ month: '2026-10' }).totalLabel()).toBe('This Month');
  });

  it('drops the month param when the only selected month is toggled off', () => {
    const component = build({ month: '2026-10' });
    component.toggleMonthFilter('2026-10');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: null } }));
  });

  it.each([
    ['toggleMonthFilter', (c: ExpenseListComponent) => c.toggleMonthFilter('2026-09')],
    ['clearMonths', (c: ExpenseListComponent) => c.clearMonths()],
    ['resetFilters', (c: ExpenseListComponent) => c.resetFilters()],
  ])('%s returns to the first page', (_name, act) => {
    const component = build({});
    component.currentPage.set(3);
    act(component);
    expect(component.currentPage()).toBe(1);
  });

  it('lists months from the current one back to January of this year', () => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date(2026, 2, 15));
    const component = build({});
    expect(component.availableMonths().map(m => m.key)).toEqual(['2026-03', '2026-02', '2026-01']);
  });
});
