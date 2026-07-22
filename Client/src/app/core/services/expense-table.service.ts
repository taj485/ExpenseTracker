import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateExpenseTableCommand, ExpenseTable } from '../models/expense-table.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ExpenseTableService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/expensetable`;

  // ── State ────────────────────────────────────────────────────────────────
  readonly tables = signal<ExpenseTable[]>([]);
  readonly loaded = signal(false);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // ── API calls ─────────────────────────────────────────────────────────────

  // API CALL: GET /api/expensetable — loads the current user's expense tables
  getTables(onSuccess?: (tables: ExpenseTable[]) => void, onError?: (msg: string) => void): void {
    this.loading.set(true);
    this.error.set(null);
    this.http.get<ExpenseTable[]>(this.apiUrl).subscribe({
      next: tables => {
        this.tables.set(tables);
        this.loaded.set(true);
        this.loading.set(false);
        onSuccess?.(tables);
      },
      error: () => {
        this.error.set('Failed to load expense tables. Please try again.');
        this.loading.set(false);
        onError?.('Failed to load expense tables. Please try again.');
      },
    });
  }

  // API CALL: POST /api/expensetable — creates a new expense table, then refreshes the list
  createTable(command: CreateExpenseTableCommand, onSuccess: () => void, onError: (msg: string) => void): void {
    this.http.post<{ id: number }>(this.apiUrl, command).subscribe({
      next: () => {
        this.getTables();
        onSuccess();
      },
      error: () => onError('Failed to create expense table. Please try again.'),
    });
  }
}
