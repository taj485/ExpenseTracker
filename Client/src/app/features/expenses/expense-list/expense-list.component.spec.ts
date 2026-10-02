import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';
import { ExpenseListComponent } from './expense-list.component';
import { ExpenseService } from '../../../core/services/expense.service';
import { ExpenseTableService } from '../../../core/services/expense-table.service';

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
});
