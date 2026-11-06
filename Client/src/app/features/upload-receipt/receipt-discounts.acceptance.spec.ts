import { TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { UploadReceiptComponent } from './upload-receipt.component';
import { ReceiptGroupComponent } from '../expenses/receipt-group/receipt-group.component';
import { ExpenseListComponent } from '../expenses/expense-list/expense-list.component';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { ImageResizeService } from '../../core/services/image-resize.service';
import { AddExpenseCommand, Expense, ExpenseCategory, ExtractedExpense } from '../../core/models/expense.model';

/**
 * PL-003 acceptance tests. The AI scan is replaced by what it returns for a clear photo of each
 * scenario's receipt (the scan rules themselves live in the Gemini prompt); everything from the
 * "Review Your Receipt" screen to the receipt page and the space list is the real UI.
 */

const HOME = { id: 1, name: 'Home', dateCreated: '2026-01-01', isCurrentUserAdmin: true, isStarred: false, memberCount: 1 };
const DATE = '2026-11-05';

interface ScanLine { description: string; unitPrice: number; quantity?: number; discount?: number; category?: ExpenseCategory }

/** What the extract-receipt API returns: the receipt discount is repeated on every item. */
function scanResult(lines: ScanLine[], receiptDiscount = 0): ExtractedExpense[] {
  return lines.map(l => ({
    unitPrice: l.unitPrice,
    quantity: l.quantity ?? 1,
    category: l.category ?? 'Food',
    description: l.description,
    date: DATE,
    merchant: 'Tesco',
    merchantWebsite: null,
    discount: l.discount ?? 0,
    receiptDiscount,
  }));
}

const S1_RECEIPT = scanResult([
  { description: 'Milk', unitPrice: 1.5, quantity: 2, discount: 0.5 },
  { description: 'Bread', unitPrice: 2 },
]);
const S2_RECEIPT = scanResult([
  { description: 'Milk', unitPrice: 1.5, quantity: 2 },
  { description: 'Bread', unitPrice: 2 },
], 1);
const S3_RECEIPT = scanResult([
  { description: 'Milk', unitPrice: 1.5, quantity: 2, discount: 0.5 },
  { description: 'Bread', unitPrice: 2 },
  { description: 'Coffee', unitPrice: 4, discount: 1 },
], 2);
const S4_RECEIPT = scanResult([
  { description: 'Milk', unitPrice: 1.5, quantity: 2 },
  { description: 'Bread', unitPrice: 2 },
]);
const S9_RECEIPT = scanResult([
  { description: 'Sandwich', unitPrice: 3 },
  { description: 'Crisps', unitPrice: 1.25 },
  { description: 'Drink', unitPrice: 1.75 },
], 3);

// ── DOM helpers ─────────────────────────────────────────────────────────────

function text(el: Element | null | undefined): string {
  return (el?.textContent ?? '').replace(/\s+/g, ' ').trim();
}

function fieldInput(root: Element, label: string): HTMLInputElement {
  const group = Array.from(root.querySelectorAll('.form-group')).find(g => text(g.querySelector('label')) === label);
  if (!group) throw new Error(`No "${label}" field`);
  return group.querySelector('input') as HTMLInputElement;
}

function typeInto(input: HTMLInputElement, value: string): void {
  input.value = value;
  input.dispatchEvent(new Event('input'));
}

function clickButton(root: Element, label: string): void {
  const button = Array.from(root.querySelectorAll('button')).find(b => text(b) === label);
  if (!button) throw new Error(`No "${label}" button`);
  (button as HTMLButtonElement).click();
}

// ── Review Your Receipt ─────────────────────────────────────────────────────

interface SavedBatch { tableIds: number[]; commands: AddExpenseCommand[]; receiptDiscount: number }

async function scanReceipt(scan: ExtractedExpense[]) {
  TestBed.resetTestingModule();
  let saved: SavedBatch | null = null;
  const addExpensesBatchToTables = vi.fn((tableIds: number[], commands: AddExpenseCommand[], _ref: string | null, receiptDiscount: number, onSuccess: (r: unknown[]) => void) => {
    saved = { tableIds, commands, receiptDiscount };
    onSuccess(tableIds.map(() => ({ addedIds: commands.map((_, i) => i + 1), errors: [] })));
  });

  TestBed.configureTestingModule({
    imports: [UploadReceiptComponent],
    providers: [
      {
        provide: ExpenseService,
        useValue: {
          extractReceipt: vi.fn((_t: number, _f: File, onSuccess: (items: ExtractedExpense[]) => void) => onSuccess(scan)),
          uploadReceiptImage: vi.fn(),
          addExpensesBatchToTables,
        },
      },
      { provide: ExpenseTableService, useValue: { tables: signal([HOME]) } },
      { provide: ImageResizeService, useValue: { resizeImage: vi.fn(async (f: File) => f) } },
    ],
  });

  const fixture = TestBed.createComponent(UploadReceiptComponent);
  const component = fixture.componentInstance;
  fixture.detectChanges();

  const photo = new File(['receipt'], 'tesco.jpg', { type: 'image/jpeg' });
  await component.onFileSelected({ target: { files: [photo], value: '' } } as unknown as Event);
  fixture.detectChanges();

  const root = fixture.nativeElement as HTMLElement;
  const items = () => Array.from(root.querySelectorAll('.extraction-item'));
  const item = (description: string) => {
    const found = items().find(i => fieldInput(i, 'Description').value === description);
    if (!found) throw new Error(`No "${description}" item`);
    return found;
  };

  return {
    fixture,
    component,
    root,
    addExpensesBatchToTables,
    itemNames: () => items().map(i => fieldInput(i, 'Description').value),
    itemDiscount: (description: string) => fieldInput(item(description), 'Discount (£)').value,
    setItemDiscount: (description: string, value: string) => { typeInto(fieldInput(item(description), 'Discount (£)'), value); fixture.detectChanges(); },
    unitPrice: (description: string) => fieldInput(item(description), 'Unit Price (£)').value,
    quantity: (description: string) => fieldInput(item(description), 'Qty').value,
    receiptDiscount: () => fieldInput(root, 'Receipt discount (£)').value,
    setReceiptDiscount: (value: string) => { typeInto(fieldInput(root, 'Receipt discount (£)'), value); fixture.detectChanges(); },
    clickAddExpenses: () => { clickButton(root, 'Add Expenses'); fixture.detectChanges(); },
    /** Clicks "Add Expenses", confirms "Home" and returns what was saved to it. */
    addExpenses: (): SavedBatch => {
      clickButton(root, 'Add Expenses');
      fixture.detectChanges();
      component.onTablesConfirmed([HOME.id]);
      if (!saved) throw new Error('Nothing was saved');
      return saved;
    },
  };
}

/** The expenses the API returns once a batch is saved as receipt 1 in "Home". */
function savedExpenses(batch: SavedBatch): Expense[] {
  return batch.commands.map((c, i) => ({
    id: i + 1,
    unitPrice: c.unitPrice,
    quantity: c.quantity,
    currency: 'GBP',
    description: c.description,
    category: c.category,
    date: `${c.date}T00:00:00Z`,
    merchantId: 1,
    merchant: c.merchant,
    merchantWebsite: null,
    receiptId: 1,
    discount: c.discount ?? 0,
    receiptDiscount: batch.receiptDiscount,
    createdByEmail: null,
    createdByCurrentUser: true,
  }));
}

// ── Receipt page ────────────────────────────────────────────────────────────

function openReceiptPage(expenses: Expense[]) {
  TestBed.resetTestingModule();
  TestBed.configureTestingModule({
    imports: [ReceiptGroupComponent],
    providers: [
      { provide: Router, useValue: { navigate: vi.fn() } },
      { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: '1', receiptId: '1' })) } },
      { provide: ExpenseService, useValue: { loadByReceiptId: vi.fn((_t: number, _r: number, onSuccess: (e: Expense[]) => void) => onSuccess(expenses)) } },
    ],
  });
  const fixture = TestBed.createComponent(ReceiptGroupComponent);
  fixture.detectChanges();
  const root = fixture.nativeElement as HTMLElement;

  const row = (label: string) => Array.from(root.querySelectorAll('.detail-fields .field')).find(f => text(f.querySelector('.field-label')) === label);
  const item = (description: string) => {
    const found = Array.from(root.querySelectorAll('.group-item')).find(i => text(i.querySelector('.group-item-description')) === description);
    if (!found) throw new Error(`No "${description}" item on the receipt page`);
    return found;
  };

  return {
    root,
    rowLabels: () => Array.from(root.querySelectorAll('.detail-fields .field-label')).map(text),
    row: (label: string) => text(row(label)?.querySelector('.field-value')),
    item: (description: string) => {
      const el = item(description);
      return {
        qty: text(el.querySelector('.group-item-qty')),
        full: text(el.querySelector('.group-item-full')),
        discount: text(el.querySelector('.group-item-discount')),
        amount: text(el.querySelector('.group-item-amount')),
      };
    },
  };
}

function expectNoItemDiscounts(page: ReturnType<typeof openReceiptPage>): void {
  expect(page.root.querySelectorAll('.group-item-discount').length).toBe(0);
}

// ── Space list ──────────────────────────────────────────────────────────────

function viewSpace(expenses: Expense[]) {
  TestBed.resetTestingModule();
  TestBed.configureTestingModule({
    imports: [ExpenseListComponent],
    providers: [
      provideHttpClient(),
      provideHttpClientTesting(),
      { provide: Router, useValue: { navigate: vi.fn() } },
      // "All months" selected.
      { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: '1' })), queryParamMap: of(convertToParamMap({ period: 'all' })) } },
      { provide: ExpenseTableService, useValue: { tables: signal([HOME]) } },
    ],
  });
  const store = TestBed.inject(ExpenseService);
  vi.spyOn(store, 'loadAll').mockImplementation(() => {});
  store.expenses.set(expenses);

  const fixture = TestBed.createComponent(ExpenseListComponent);
  fixture.detectChanges();
  const root = fixture.nativeElement as HTMLElement;
  const card = () => root.querySelector('.receipt-card');

  return {
    cardTotal: () => text(card()?.querySelector('.receipt-total')),
    rowAmount: (description: string) => {
      const row = Array.from(card()?.querySelectorAll('.receipt-item') ?? []).find(r => text(r.querySelector('.item-name')) === description);
      return text(row?.querySelector('.item-amount'));
    },
    statCard: (label: string) => {
      const stat = Array.from(root.querySelectorAll('.stat-card')).find(s => text(s.querySelector('.st-label')) === label);
      return text(stat?.querySelector('.st-value'));
    },
  };
}

// ── Scenarios ───────────────────────────────────────────────────────────────

describe('PL-003 Receipt discounts picked up from the scan', () => {
  it('S1_item_discount_is_picked_up_and_shown_on_the_receipt_page', async () => {
    const review = await scanReceipt(S1_RECEIPT);

    expect(review.itemNames()).toEqual(['Milk', 'Bread']);
    expect(review.unitPrice('Milk')).toBe('1.5');
    expect(review.quantity('Milk')).toBe('2');
    expect(review.itemDiscount('Milk')).toBe('0.5');
    expect(review.itemDiscount('Bread')).toBe('');
    expect(review.itemNames()).not.toContain('Clubcard Price');
    expect(review.receiptDiscount()).toBe('');

    const page = openReceiptPage(savedExpenses(review.addExpenses()));

    expect(page.row('Subtotal')).toBe('£5.00');
    expect(page.row('Item discounts')).toBe('−£0.50');
    expect(page.row('Total')).toBe('£4.50');
    expect(page.rowLabels()).not.toContain('Receipt discount');
    expect(page.item('Milk')).toEqual({ qty: '2 × £1.50', full: '£3.00', discount: '−£0.50', amount: '£2.50' });
    expect(page.item('Bread')).toEqual({ qty: '', full: '', discount: '', amount: '£2.00' });
  });

  it('S2_whole_receipt_discount_is_picked_up_and_shown_on_the_receipt_page', async () => {
    const review = await scanReceipt(S2_RECEIPT);

    expect(review.itemNames()).toEqual(['Milk', 'Bread']);
    expect(review.itemDiscount('Milk')).toBe('');
    expect(review.itemDiscount('Bread')).toBe('');
    expect(review.itemNames()).not.toContain('Coupon');
    expect(review.receiptDiscount()).toBe('1');

    const page = openReceiptPage(savedExpenses(review.addExpenses()));

    expect(page.row('Subtotal')).toBe('£5.00');
    expect(page.row('Receipt discount')).toBe('−£1.00');
    expect(page.row('Total')).toBe('£4.00');
    expect(page.rowLabels()).not.toContain('Item discounts');
    expectNoItemDiscounts(page);
  });

  it('S3_item_and_receipt_discounts_together_without_counting_savings_twice', async () => {
    const review = await scanReceipt(S3_RECEIPT);

    expect(review.itemNames()).toEqual(['Milk', 'Bread', 'Coffee']);
    expect(review.itemDiscount('Milk')).toBe('0.5');
    expect(review.itemDiscount('Bread')).toBe('');
    expect(review.itemDiscount('Coffee')).toBe('1');
    expect(review.receiptDiscount()).toBe('2');
    for (const name of ['Clubcard Price', 'Multibuy saving', 'Total savings']) {
      expect(review.itemNames()).not.toContain(name);
    }

    const page = openReceiptPage(savedExpenses(review.addExpenses()));

    expect(page.row('Subtotal')).toBe('£9.00');
    expect(page.row('Item discounts')).toBe('−£1.50');
    expect(page.row('Receipt discount')).toBe('−£2.00');
    expect(page.row('Total')).toBe('£5.50');
    expect(page.item('Milk')).toMatchObject({ full: '£3.00', discount: '−£0.50', amount: '£2.50' });
    expect(page.item('Coffee')).toMatchObject({ full: '£4.00', discount: '−£1.00', amount: '£3.00' });
    expect(page.item('Bread')).toMatchObject({ full: '', discount: '', amount: '£2.00' });
  });

  it('S4_receipt_with_no_discount_lines_looks_the_same_as_today', async () => {
    const review = await scanReceipt(S4_RECEIPT);

    expect(review.itemDiscount('Milk')).toBe('');
    expect(review.itemDiscount('Bread')).toBe('');
    expect(review.receiptDiscount()).toBe('');

    const expenses = savedExpenses(review.addExpenses());
    const page = openReceiptPage(expenses);

    expect(page.rowLabels()).toEqual(['Total']);
    expect(page.row('Total')).toBe('£5.00');
    expectNoItemDiscounts(page);

    expect(viewSpace(expenses).cardTotal()).toBe('£5.00');
  });

  it('S5_space_list_and_totals_count_the_amount_after_discounts', async () => {
    const review = await scanReceipt(S3_RECEIPT);
    const space = viewSpace(savedExpenses(review.addExpenses()));

    expect(space.cardTotal()).toBe('£5.50');
    expect(space.rowAmount('Milk')).toBe('£2.50');
    expect(space.rowAmount('Bread')).toBe('£2.00');
    expect(space.rowAmount('Coffee')).toBe('£3.00');
    expect(space.statCard('Total')).toBe('£5.50');
  });

  it('S6_correcting_a_discount_saves_the_corrected_value', async () => {
    const review = await scanReceipt(S3_RECEIPT);
    expect(review.itemDiscount('Coffee')).toBe('1');
    expect(review.receiptDiscount()).toBe('2');

    review.setItemDiscount('Coffee', '0.75');
    review.setReceiptDiscount('1.50');
    const page = openReceiptPage(savedExpenses(review.addExpenses()));

    expect(page.row('Subtotal')).toBe('£9.00');
    expect(page.row('Item discounts')).toBe('−£1.25');
    expect(page.row('Receipt discount')).toBe('−£1.50');
    expect(page.row('Total')).toBe('£6.25');
    expect(page.item('Coffee')).toMatchObject({ full: '£4.00', discount: '−£0.75', amount: '£3.25' });
  });

  it.each([
    { discount: '3.50', message: "Discount can't be more than the item total (£3.00)." },
    { discount: '-0.50', message: "Discount can't be negative." },
  ])('S7_rejects_invalid_item_discount_$discount', async ({ discount, message }) => {
    const review = await scanReceipt(S1_RECEIPT);
    expect(review.unitPrice('Milk')).toBe('1.5');
    expect(review.quantity('Milk')).toBe('2');

    review.setItemDiscount('Milk', discount);
    review.clickAddExpenses();

    expect(text(review.root)).toContain(message);
    expect(text(review.root.querySelector('.upload-title'))).toBe('Review Your Receipt');
    expect(review.addExpensesBatchToTables).not.toHaveBeenCalled();
  });

  it('S8_rejects_receipt_discount_larger_than_total_after_item_discounts', async () => {
    const review = await scanReceipt(S1_RECEIPT);
    expect(review.itemDiscount('Milk')).toBe('0.5');

    review.setReceiptDiscount('5.00');
    review.clickAddExpenses();

    expect(text(review.root)).toContain("Receipt discount can't be more than the receipt total (£4.50).");
    expect(text(review.root.querySelector('.upload-title'))).toBe('Review Your Receipt');
    expect(review.addExpensesBatchToTables).not.toHaveBeenCalled();
  });

  it('S9_multi_item_discount_goes_to_the_receipt_discount_and_adds_to_a_coupon', async () => {
    const review = await scanReceipt(S9_RECEIPT);

    expect(review.itemNames()).toEqual(['Sandwich', 'Crisps', 'Drink']);
    for (const name of ['Sandwich', 'Crisps', 'Drink']) expect(review.itemDiscount(name)).toBe('');
    expect(review.itemNames()).not.toContain('Meal Deal saving');
    expect(review.itemNames()).not.toContain('Coupon');
    expect(review.receiptDiscount()).toBe('3');

    const page = openReceiptPage(savedExpenses(review.addExpenses()));

    expect(page.row('Subtotal')).toBe('£6.00');
    expect(page.row('Receipt discount')).toBe('−£3.00');
    expect(page.row('Total')).toBe('£3.00');
    expect(page.rowLabels()).not.toContain('Item discounts');
    expect(page.item('Sandwich').amount).toBe('£3.00');
    expect(page.item('Crisps').amount).toBe('£1.25');
    expect(page.item('Drink').amount).toBe('£1.75');
    expectNoItemDiscounts(page);
  });
});
