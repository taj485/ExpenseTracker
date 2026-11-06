import { TestBed, ComponentFixture } from '@angular/core/testing';
import { signal } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, ParamMap, Params, Router, convertToParamMap } from '@angular/router';
import { BehaviorSubject, of } from 'rxjs';
import { ExpenseListComponent } from './expense-list.component';
import { ExpenseTableService } from '../../../core/services/expense-table.service';
import { Expense } from '../../../core/models/expense.model';

// Acceptance tests for PL-001: the list has no Today / This week / Last week chips or day tabs,
// and opens on All months. Background: today is Friday 6 November 2026.

const HOME_ID = 1;
const EMPTY_ID = 2;

const homeExpenses = [
  { id: 1, date: '2026-11-05T00:00:00', description: 'Milk', unitPrice: 1.5, quantity: 1, category: 'Food', receiptId: null },
  { id: 2, date: '2026-10-12T00:00:00', description: 'Bread', unitPrice: 2, quantity: 1, category: 'Food', receiptId: null },
  { id: 3, date: '2026-03-01T00:00:00', description: 'Coffee', unitPrice: 3.25, quantity: 1, category: 'Food', receiptId: null },
] as Expense[];

interface Page {
  fixture: ComponentFixture<ExpenseListComponent>;
  el: HTMLElement;
}

function open(tableId: number, expenses: Expense[], query: Params = {}): Page {
  let params: Params = { ...query };
  const queryParamMap = new BehaviorSubject<ParamMap>(convertToParamMap(params));
  // Stands in for the router: applies merged query params so the component sees the new URL.
  const navigate = vi.fn((_commands: unknown[], extras?: { queryParams?: Params }) => {
    params = { ...params, ...extras?.queryParams };
    for (const key of Object.keys(params)) if (params[key] == null) delete params[key];
    queryParamMap.next(convertToParamMap(params));
    return Promise.resolve(true);
  });

  TestBed.configureTestingModule({
    providers: [
      provideHttpClient(),
      provideHttpClientTesting(),
      { provide: Router, useValue: { navigate } },
      { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ tableId: String(tableId) })), queryParamMap } },
      {
        provide: ExpenseTableService,
        useValue: {
          tables: signal([
            { id: HOME_ID, name: 'Home', isStarred: false, isCurrentUserAdmin: true, memberCount: 1 },
            { id: EMPTY_ID, name: 'Empty', isStarred: false, isCurrentUserAdmin: true, memberCount: 1 },
          ]),
        },
      },
    ],
  });

  const fixture = TestBed.createComponent(ExpenseListComponent);
  fixture.detectChanges();
  TestBed.inject(HttpTestingController)
    .expectOne(req => req.url.endsWith(`/expensetable/${tableId}/expenses`))
    .flush(expenses);
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

const text = (e: Element) => e.textContent?.trim() ?? '';

function dateButtons(page: Page): string[] {
  return Array.from(page.el.querySelectorAll('[aria-label="Filter by date"] button')).map(text);
}

function selectedDateChips(page: Page): string[] {
  return Array.from(page.el.querySelectorAll('[aria-label="Filter by date"] .is-selected')).map(text);
}

function statCards(page: Page): { label: string; value: string }[] {
  return Array.from(page.el.querySelectorAll('.stat-card')).map(card => ({
    label: text(card.querySelector('.st-label')!),
    value: text(card.querySelector('.st-value')!),
  }));
}

function expenseNames(page: Page): string[] {
  return Array.from(page.el.querySelectorAll('.item-name')).map(text);
}

function dayTabs(page: Page): Element | null {
  return page.el.querySelector('[aria-label="Filter by day"]');
}

function click(page: Page, groupLabel: string, label: string): void {
  const button = Array.from(page.el.querySelectorAll<HTMLButtonElement>(`[aria-label="${groupLabel}"] button`))
    .find(b => text(b) === label || text(b).endsWith(` ${label}`));
  if (!button) throw new Error(`No "${label}" button in ${groupLabel}`);
  button.click();
  page.fixture.detectChanges();
}

function entries(page: Page): string {
  return statCards(page).find(c => c.label === 'Entries')!.value;
}

describe('PL-001 expense list without week or day filters', () => {
  beforeEach(() => {
    vi.useFakeTimers({ toFake: ['Date'] });
    vi.setSystemTime(new Date(2026, 10, 6, 12, 0, 0));
  });

  afterEach(() => {
    TestBed.inject(HttpTestingController).verify();
    vi.useRealTimers();
  });

  it('S1_opens_on_all_months_with_no_week_or_day_filters', () => {
    const page = open(HOME_ID, homeExpenses);
    const buttons = dateButtons(page);

    expect(selectedDateChips(page)).toEqual(['All months']);
    expect(buttons).not.toContain('Today');
    expect(buttons).not.toContain('This week');
    expect(buttons).not.toContain('Last week');
    // All months, then Nov 2026 down to Jan 2026, then Reset.
    expect(buttons[0]).toBe('All months');
    expect(buttons[1]).toBe('Nov 2026');
    expect(buttons[11]).toBe('Jan 2026');
    expect(buttons[12]).toBe('Reset');
    expect(buttons).toHaveLength(13);
    expect(dayTabs(page)).toBeNull();
  });

  it('S2_lists_every_months_expenses_by_default', () => {
    const page = open(HOME_ID, homeExpenses);

    expect(expenseNames(page).sort()).toEqual(['Bread', 'Coffee', 'Milk']);
    expect(statCards(page)).toEqual([
      { label: 'Total', value: '£6.75' },
      { label: 'This Month', value: '£1.50' },
      { label: 'Entries', value: '3' },
    ]);
  });

  it('S3_old_link_with_week_and_day_filter_opens_on_all_months', () => {
    const page = open(HOME_ID, homeExpenses, { period: 'this-week', weekday: 'thu' });

    expect(selectedDateChips(page)).toEqual(['All months']);
    expect(expenseNames(page).sort()).toEqual(['Bread', 'Coffee', 'Milk']);
    expect(dayTabs(page)).toBeNull();
  });

  for (const period of ['today', 'this-week', 'last-week']) {
    it(`S4_old_link_with_period_${period}_falls_back_to_all_months`, () => {
      const page = open(HOME_ID, homeExpenses, { period });

      expect(selectedDateChips(page)).toEqual(['All months']);
      expect(entries(page)).toBe('3');
    });
  }

  it('S5_month_filtering_still_works', () => {
    const page = open(HOME_ID, homeExpenses);

    click(page, 'Filter by date', 'Oct 2026');
    expect(selectedDateChips(page)).toEqual(['Oct 2026']);
    expect(expenseNames(page)).toEqual(['Bread']);
    expect(statCards(page)[0]).toEqual({ label: 'This Month', value: '£2.00' });

    click(page, 'Filter by date', 'Oct 2026');
    expect(selectedDateChips(page)).toEqual(['All months']);
    expect(entries(page)).toBe('3');
  });

  it('S6_reset_goes_back_to_all_months', () => {
    const page = open(HOME_ID, homeExpenses);
    click(page, 'Filter by date', 'Mar 2026');
    click(page, 'Filter by category', 'Food');
    expect(selectedDateChips(page)).toEqual(['Mar 2026']);

    click(page, 'Filter by date', 'Reset');

    expect(selectedDateChips(page)).toEqual(['All months']);
    const selectedCategories = Array.from(page.el.querySelectorAll('[aria-label="Filter by category"] .is-selected')).map(text);
    expect(selectedCategories).toEqual(['All categories']);
    expect(entries(page)).toBe('3');
  });

  it('S7_space_with_no_expenses', () => {
    const page = open(EMPTY_ID, []);

    expect(selectedDateChips(page)).toEqual(['All months']);
    expect(statCards(page)[0]).toEqual({ label: 'Total', value: '£0.00' });
    expect(entries(page)).toBe('0');
  });
});
