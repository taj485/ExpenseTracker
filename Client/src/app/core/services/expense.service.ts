import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { AddExpenseCommand, AddExpensesBatchResult, CategoryStat, Expense, ExpenseCategory, ExtractedExpense, MerchantStat, UpdateExpenseCommand } from '../models/expense.model';
import { environment } from '../../../environments/environment';
import { parseFilenameFromContentDisposition } from '../utils/download.utils';
import { MonthOption, currentMonthKey, formatMonthKey, monthKeyOf, monthKeysBack } from '../utils/date.utils';
import { expenseTotal } from '../utils/expense.utils';

/** How far back the dashboard month picker reaches, in months before the current one. */
const MONTHS_SELECTABLE = 12;

/**
 * Slices in the merchant donut before the tail is folded into "Other". Six segments is the
 * point past which a part-to-whole ring stops being readable at a glance.
 */
const TOP_MERCHANTS = 5;

/** Shown for expenses with no merchant recorded. */
const UNKNOWN_MERCHANT = 'Unknown';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private readonly http = inject(HttpClient);

  private tableUrl(tableId: number): string {
    return `${environment.apiUrl}/expensetable/${tableId}/expenses`;
  }

  private currentTableId: number | null = null;

  // ── State ────────────────────────────────────────────────────────────────
  readonly expenses = signal<Expense[]>([]);
  readonly loading  = signal(false);
  readonly error    = signal<string | null>(null);

  // ── Month selection ───────────────────────────────────────────────────────
  /** Month the dashboard is showing, as 'YYYY-MM'. Defaults to the current month. */
  readonly selectedMonth = signal<string>(currentMonthKey());

  /**
   * The last 13 months (current plus 12 back), offered whether or not they hold expenses.
   * Months outside that window are added when they contain expenses, so older data stays
   * reachable rather than silently dropping off the dashboard.
   */
  readonly availableMonths = computed((): MonthOption[] => {
    const keys = new Set(monthKeysBack(MONTHS_SELECTABLE));
    for (const expense of this.expenses()) keys.add(monthKeyOf(expense.date));
    keys.add(this.selectedMonth());

    // 'YYYY-MM' sorts lexicographically in date order, so this is newest-first.
    return [...keys]
      .sort()
      .reverse()
      .map(key => ({ key, label: formatMonthKey(key) }));
  });

  // ── Computed dashboard values ─────────────────────────────────────────────
  private readonly selectedMonthExpenses = computed(() =>
    this.expenses().filter(e => monthKeyOf(e.date) === this.selectedMonth())
  );

  readonly selectedMonthSpent = computed(() =>
    this.selectedMonthExpenses().reduce((sum, e) => sum + expenseTotal(e), 0)
  );

  readonly transactionCount = computed(() => this.selectedMonthExpenses().length);

  /**
   * Always the current calendar month, independent of selectedMonth. The expense list shows
   * this beside its own filters, where "This Month" has to keep meaning today's month.
   */
  readonly currentMonthSpent = computed(() => {
    const month = currentMonthKey();
    return this.expenses()
      .filter(e => monthKeyOf(e.date) === month)
      .reduce((sum, e) => sum + expenseTotal(e), 0);
  });

  readonly topCategory = computed((): { name: ExpenseCategory; total: number } | null => {
    const expenses = this.selectedMonthExpenses();
    if (expenses.length === 0) return null;

    const totals = new Map<ExpenseCategory, number>();
    for (const e of expenses) {
      totals.set(e.category, (totals.get(e.category) ?? 0) + expenseTotal(e));
    }

    let topName: ExpenseCategory = expenses[0].category;
    let topTotal = 0;
    for (const [name, total] of totals) {
      if (total > topTotal) { topName = name; topTotal = total; }
    }

    return { name: topName, total: topTotal };
  });

  readonly categoryBreakdown = computed((): CategoryStat[] => {
    const expenses = this.selectedMonthExpenses();
    const total = this.selectedMonthSpent();
    if (expenses.length === 0) return [];

    const totals = new Map<ExpenseCategory, { total: number; count: number }>();
    for (const e of expenses) {
      const existing = totals.get(e.category) ?? { total: 0, count: 0 };
      totals.set(e.category, { total: existing.total + expenseTotal(e), count: existing.count + 1 });
    }

    return Array.from(totals.entries())
      .map(([category, { total: catTotal, count }]) => ({
        category,
        total: catTotal,
        count,
        percentage: total > 0 ? Math.round((catTotal / total) * 100) : 0,
      }))
      .sort((a, b) => b.total - a.total);
  });

  /** Spend per merchant for the selected month: the top few, then everything else as "Other". */
  readonly merchantBreakdown = computed((): MerchantStat[] => {
    const expenses = this.selectedMonthExpenses();
    const total = this.selectedMonthSpent();
    if (expenses.length === 0) return [];

    // Group on merchantId so two spellings of one shop can never split; fall back to the
    // name for anything not yet resolved to a reference-table row.
    const groups = new Map<string, { merchant: string; website: string | null; total: number; count: number }>();

    for (const e of expenses) {
      const key = e.merchantId !== null ? `id:${e.merchantId}` : `name:${e.merchant ?? ''}`;
      const existing = groups.get(key);

      if (existing) {
        existing.total += expenseTotal(e);
        existing.count += 1;
      } else {
        groups.set(key, {
          merchant: e.merchant ?? UNKNOWN_MERCHANT,
          website: e.merchantWebsite,
          total: expenseTotal(e),
          count: 1,
        });
      }
    }

    const share = (amount: number) => (total > 0 ? Math.round((amount / total) * 100) : 0);
    const ranked = [...groups.values()].sort((a, b) => b.total - a.total);
    const top = ranked.slice(0, TOP_MERCHANTS);
    const tail = ranked.slice(TOP_MERCHANTS);

    const stats: MerchantStat[] = top.map(g => ({
      merchant: g.merchant,
      website: g.website,
      total: g.total,
      count: g.count,
      percentage: share(g.total),
      isOther: false,
    }));

    if (tail.length > 0) {
      const tailTotal = tail.reduce((sum, g) => sum + g.total, 0);
      stats.push({
        merchant: `Other (${tail.length})`,
        website: null,
        total: tailTotal,
        count: tail.reduce((sum, g) => sum + g.count, 0),
        percentage: share(tailTotal),
        isOther: true,
      });
    }

    return stats;
  });

  // ── API calls ─────────────────────────────────────────────────────────────

  // API CALL: GET /api/expensetable/{tableId}/expenses — loads a table's expenses into signal
  loadAll(tableId: number): void {
    // Switching table starts fresh on the current month; a plain refresh after adding or
    // editing an expense must not yank the user out of the month they were looking at.
    if (this.currentTableId !== tableId) this.selectedMonth.set(currentMonthKey());

    this.currentTableId = tableId;
    this.loading.set(true);
    this.error.set(null);
    this.http.get<Expense[]>(this.tableUrl(tableId)).subscribe({
      next: expenses => {
        this.expenses.set(expenses);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load expenses. Please try again.');
        this.loading.set(false);
      },
    });
  }

  // API CALL: GET /api/expensetable/{tableId}/expenses/{id} — fetch single expense by id
  loadById(tableId: number, id: number, onSuccess: (e: Expense) => void, onError: () => void): void {
    const cached = this.expenses().find(e => e.id === id);
    if (cached) { onSuccess(cached); return; }

    this.http.get<Expense>(`${this.tableUrl(tableId)}/${id}`).subscribe({
      next: expense => {
        this.expenses.update(list => [...list, expense]);
        onSuccess(expense);
      },
      error: onError,
    });
  }

  // API CALL: POST /api/expensetable/{tableId}/expenses for each selected table — adds one expense to every checked table
  addExpenseToTables(
    tableIds: number[],
    command: Omit<AddExpenseCommand, 'expenseTableId'>,
    onSuccess: () => void,
    onError: (msg: string) => void,
  ): void {
    forkJoin(tableIds.map(tableId =>
      this.http.post<{ id: number }>(this.tableUrl(tableId), { ...command, expenseTableId: tableId })
    )).subscribe({
      next: () => {
        this.refreshIfCurrentTableAffected(tableIds);
        onSuccess();
      },
      error: () => onError('Failed to add expense to one or more tables. Please try again.'),
    });
  }

  // API CALL: POST /api/expensetable/{tableId}/expenses/batch for each selected table — adds multiple expenses to every checked table
  addExpensesBatchToTables(
    tableIds: number[],
    commands: AddExpenseCommand[],
    receiptImageReference: string | null,
    onSuccess: (results: AddExpensesBatchResult[]) => void,
    onError: (msg: string) => void,
  ): void {
    forkJoin(tableIds.map(tableId =>
      this.http.post<AddExpensesBatchResult>(`${this.tableUrl(tableId)}/batch`, {
        expenseTableId: tableId,
        items: commands,
        receiptImageReference,
      })
    )).subscribe({
      next: (results) => {
        this.refreshIfCurrentTableAffected(tableIds);
        onSuccess(results);
      },
      error: () => onError('Failed to add expenses to one or more tables. Please try again.'),
    });
  }

  private refreshIfCurrentTableAffected(tableIds: number[]): void {
    if (this.currentTableId !== null && tableIds.includes(this.currentTableId)) {
      this.loadAll(this.currentTableId);
    }
  }

  // API CALL: POST /api/expensetable/{tableId}/expenses/extract-receipt — extracts structured line items from a receipt photo (multipart upload)
  extractReceipt(
    tableId: number,
    file: File,
    onSuccess: (items: ExtractedExpense[]) => void,
    onError: (msg: string) => void,
  ): void {
    const formData = new FormData();
    formData.append('file', file);

    this.http.post<ExtractedExpense[]>(`${this.tableUrl(tableId)}/extract-receipt`, formData).subscribe({
      next: onSuccess,
      error: () => onError("Couldn't read this receipt. Try a different photo or enter it manually."),
    });
  }

  // API CALL: POST /api/expensetable/{tableId}/expenses/receipt-image — uploads the receipt photo to blob storage, returns a reference for later download
  uploadReceiptImage(
    tableId: number,
    file: File,
    onSuccess: (imageReference: string) => void,
    onError: (msg: string) => void,
  ): void {
    const formData = new FormData();
    formData.append('file', file);

    this.http.post<{ imageReference: string }>(`${this.tableUrl(tableId)}/receipt-image`, formData).subscribe({
      next: (result) => onSuccess(result.imageReference),
      error: () => onError('Failed to upload receipt image.'),
    });
  }

  // API CALL: GET /api/expensetable/{tableId}/expenses/by-receipt/{receiptId}/image — downloads the receipt image as a blob
  downloadReceiptImage(
    tableId: number,
    receiptId: number,
    onSuccess: (blob: Blob, filename: string) => void,
    onError: (msg: string) => void,
  ): void {
    this.http.get(`${this.tableUrl(tableId)}/by-receipt/${receiptId}/image`, {
      observe: 'response',
      responseType: 'blob',
    }).subscribe({
      next: (response) => {
        const filename = parseFilenameFromContentDisposition(response.headers.get('content-disposition'))
          ?? `receipt-${receiptId}.jpg`;
        onSuccess(response.body as Blob, filename);
      },
      error: () => onError('No image is available for this receipt.'),
    });
  }

  // API CALL: PUT /api/expensetable/{tableId}/expenses/{id} — update an expense, patches the signal on success
  updateExpense(tableId: number, id: number, command: UpdateExpenseCommand, onSuccess: () => void, onError: (msg: string) => void): void {
    this.http.put<void>(`${this.tableUrl(tableId)}/${id}`, { id, ...command }).subscribe({
      next: () => {
        this.expenses.update(list => list.map(e => {
          if (e.id !== id) return e;
          // The server resolves the merchant name to a reference-table row, so the id and website
          // we hold are only valid while the name is unchanged. Clearing them makes the logo fall
          // back to domain guessing rather than showing the previous merchant's brand.
          const merchantChanged = e.merchant !== command.merchant;
          return {
            ...e,
            ...command,
            merchantId: merchantChanged ? null : e.merchantId,
            merchantWebsite: merchantChanged ? null : e.merchantWebsite,
          };
        }));
        onSuccess();
      },
      error: () => onError('Failed to update expense. Please try again.'),
    });
  }

  // API CALL: DELETE /api/expensetable/{tableId}/expenses/{id} — delete an expense, removes it from the signal on success
  deleteExpense(tableId: number, id: number, onSuccess: () => void, onError: (msg: string) => void): void {
    this.http.delete<void>(`${this.tableUrl(tableId)}/${id}`).subscribe({
      next: () => {
        this.expenses.update(list => list.filter(e => e.id !== id));
        onSuccess();
      },
      error: () => onError('Failed to delete expense. Please try again.'),
    });
  }

  // API CALL: GET /api/expensetable/{tableId}/expenses/by-receipt/{receiptId} — fetch all expenses sharing a receipt group
  loadByReceiptId(tableId: number, receiptId: number, onSuccess: (items: Expense[]) => void, onError: (msg: string) => void): void {
    this.http.get<Expense[]>(`${this.tableUrl(tableId)}/by-receipt/${receiptId}`).subscribe({
      next: (items) => {
        const ids = new Set(items.map(i => i.id));
        this.expenses.update(list => [...list.filter(e => !ids.has(e.id)), ...items]);
        onSuccess(items);
      },
      error: () => onError('Failed to load receipt group. Please try again.'),
    });
  }

  // API CALL: DELETE /api/expensetable/{tableId}/expenses/{id} for every id — deletes all expenses in a receipt group
  deleteExpenses(tableId: number, ids: number[], onSuccess: () => void, onError: (msg: string) => void): void {
    if (ids.length === 0) { onSuccess(); return; }

    forkJoin(ids.map(id => this.http.delete<void>(`${this.tableUrl(tableId)}/${id}`))).subscribe({
      next: () => {
        const idSet = new Set(ids);
        this.expenses.update(list => list.filter(e => !idSet.has(e.id)));
        onSuccess();
      },
      error: () => onError('Failed to delete all expenses in this group. Some items may have been removed — please refresh.'),
    });
  }
}
