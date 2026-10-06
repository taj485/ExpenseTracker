import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { ExpenseListComponent } from './expense-list.component';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { Expense } from '../../../core/models/expense.model';
import { todayLocalISODate } from '../../../core/utils/date.utils';

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

  it('turns on the Today filter and clears any month', () => {
    component.toggleToday();
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({
      queryParams: { day: 'today', month: null },
      queryParamsHandling: 'merge',
    }));
  });

  it('turns the Today filter off when a month is picked', () => {
    component.onMonthChange('2026-09');
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: '2026-09', day: null } }));
  });

  it('clears the Today filter on reset', () => {
    component.resetFilters();
    expect(navigate).toHaveBeenCalledWith([], expect.objectContaining({ queryParams: { month: null, day: null, category: null } }));
  });
});

describe('ExpenseListComponent with ?day=today', () => {
  it('shows only expenses dated today', () => {
    const today = todayLocalISODate();
    const expenses = [
      { id: 1, date: `${today}T00:00:00`, unitPrice: 2, quantity: 1, category: 'Food' },
      { id: 2, date: '2020-01-01T00:00:00', unitPrice: 5, quantity: 1, category: 'Food' },
    ] as Expense[];

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: { navigate: vi.fn() } },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: '7' })), queryParamMap: of(convertToParamMap({ day: 'today' })) } },
        { provide: ExpenseService, useValue: { expenses: signal(expenses), loadAll: vi.fn() } },
        { provide: ExpenseTableService, useValue: { tables: signal([]) } },
      ],
    });
    const component = TestBed.runInInjectionContext(() => new ExpenseListComponent());

    expect(component.isTodayFilter()).toBe(true);
    expect(component.filteredExpenses().map(e => e.id)).toEqual([1]);
    expect(component.filteredTotal()).toBe(2);
  });
});
