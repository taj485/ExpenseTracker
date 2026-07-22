import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { AddExpenseCommand, AddExpensesBatchResult, CategoryStat, Expense, ExpenseCategory, ExtractedExpense, UpdateExpenseCommand } from '../models/expense.model';
import { environment } from '../../../environments/environment';

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

  // ── Computed dashboard values ─────────────────────────────────────────────
  private readonly thisMonthExpenses = computed(() => {
    const now = new Date();
    return this.expenses().filter(e => {
      const d = new Date(e.date);
      return d.getFullYear() === now.getFullYear() && d.getMonth() === now.getMonth();
    });
  });

  readonly thisMonthSpent = computed(() =>
    this.thisMonthExpenses().reduce((sum, e) => sum + e.amount, 0)
  );

  readonly transactionCount = computed(() => this.thisMonthExpenses().length);

  readonly topCategory = computed((): { name: ExpenseCategory; total: number } | null => {
    const expenses = this.thisMonthExpenses();
    if (expenses.length === 0) return null;

    const totals = new Map<ExpenseCategory, number>();
    for (const e of expenses) {
      totals.set(e.category, (totals.get(e.category) ?? 0) + e.amount);
    }

    let topName: ExpenseCategory = expenses[0].category;
    let topTotal = 0;
    for (const [name, total] of totals) {
      if (total > topTotal) { topName = name; topTotal = total; }
    }

    return { name: topName, total: topTotal };
  });

  readonly categoryBreakdown = computed((): CategoryStat[] => {
    const expenses = this.thisMonthExpenses();
    const total = this.thisMonthSpent();
    if (expenses.length === 0) return [];

    const totals = new Map<ExpenseCategory, { total: number; count: number }>();
    for (const e of expenses) {
      const existing = totals.get(e.category) ?? { total: 0, count: 0 };
      totals.set(e.category, { total: existing.total + e.amount, count: existing.count + 1 });
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

  // ── API calls ─────────────────────────────────────────────────────────────

  // API CALL: GET /api/expensetable/{tableId}/expenses — loads a table's expenses into signal
  loadAll(tableId: number): void {
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
    onSuccess: (results: AddExpensesBatchResult[]) => void,
    onError: (msg: string) => void,
  ): void {
    forkJoin(tableIds.map(tableId =>
      this.http.post<AddExpensesBatchResult>(`${this.tableUrl(tableId)}/batch`, commands)
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

  // API CALL: PUT /api/expensetable/{tableId}/expenses/{id} — update an expense, patches the signal on success
  updateExpense(tableId: number, id: number, command: UpdateExpenseCommand, onSuccess: () => void, onError: (msg: string) => void): void {
    this.http.put<void>(`${this.tableUrl(tableId)}/${id}`, { id, ...command }).subscribe({
      next: () => {
        this.expenses.update(list => list.map(e => e.id === id ? { ...e, ...command } : e));
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
