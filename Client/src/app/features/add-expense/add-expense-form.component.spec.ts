import { TestBed } from '@angular/core/testing';
import { AddExpenseFormComponent } from './add-expense-form.component';
import { ExpenseService } from '../../core/services/expense.service';

describe('AddExpenseFormComponent quantity', () => {
  let component: AddExpenseFormComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: ExpenseService, useValue: { addExpenseToTables: vi.fn() } }],
    });
    component = TestBed.runInInjectionContext(() => new AddExpenseFormComponent());
    component.unitPrice.set('5');
    component.description.set('Chicken');
  });

  it('accepts a fractional weight in kg and moves on to picking tables', () => {
    component.quantity.set('0.612');
    component.unit.set('Kg');

    component.onSubmit();

    expect(component.formError()).toBeNull();
    expect(component.step()).toBe('select-tables');
  });

  it('rejects a fractional quantity of items', () => {
    component.quantity.set('1.5');
    component.unit.set('Item');

    component.onSubmit();

    expect(component.formError()).toBe('Quantity must be a whole number of at least 1.');
    expect(component.step()).toBe('form');
  });

  it('sends the weight and unit to the service', () => {
    const service = TestBed.inject(ExpenseService) as unknown as { addExpenseToTables: ReturnType<typeof vi.fn> };
    component.quantity.set('0.612');
    component.unit.set('Kg');
    component.onSubmit();

    component.onTablesConfirmed([7]);

    expect(service.addExpenseToTables).toHaveBeenCalledWith(
      [7],
      expect.objectContaining({ quantity: 0.612, unit: 'Kg', unitPrice: 5 }),
      expect.any(Function),
      expect.any(Function),
    );
  });
});
