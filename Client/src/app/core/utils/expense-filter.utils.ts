import { Expense, ExpenseCategory } from '../models/expense.model';
import { ALL_CATEGORIES } from './category.utils';

/**
 * The `category` query param holds one or more categories, most recently selected first:
 * `?category=Food,Health`. Returns them in that order, dropping unknown and duplicate values.
 */
export function parseCategoryParam(value: string | null): ExpenseCategory[] {
  const requested = (value ?? '').split(',').map(c => c.trim());
  return requested.filter(
    (c, index): c is ExpenseCategory =>
      ALL_CATEGORIES.includes(c as ExpenseCategory) && requested.indexOf(c) === index,
  );
}

/** Removes the category if selected; otherwise adds it at the front so the latest pick leads. */
export function toggleCategory(selected: ExpenseCategory[], category: ExpenseCategory): ExpenseCategory[] {
  return selected.includes(category) ? selected.filter(c => c !== category) : [category, ...selected];
}

/**
 * The `month` query param holds one or more 'YYYY-MM' keys: `?month=2026-10,2026-09`.
 * Returns them newest first, dropping malformed and duplicate values.
 */
export function parseMonthParam(value: string | null): string[] {
  return newestFirst((value ?? '').split(',').map(m => m.trim()).filter(m => MONTH_KEY.test(m)));
}

/** Removes the month if selected; otherwise adds it. Months stay newest first, unlike categories. */
export function toggleMonth(selected: string[], month: string): string[] {
  return selected.includes(month) ? selected.filter(m => m !== month) : newestFirst([...selected, month]);
}

const MONTH_KEY = /^\d{4}-\d{2}$/;

/** De-duplicated, newest first. 'YYYY-MM' keys sort correctly as strings. */
function newestFirst(months: string[]): string[] {
  return [...new Set(months)].sort().reverse();
}

/** Chip order: selected categories first (latest first), then the rest in their usual order. */
export function orderCategoryChips(selected: ExpenseCategory[]): ExpenseCategory[] {
  return [...selected, ...ALL_CATEGORIES.filter(c => !selected.includes(c))];
}

/** Case-insensitive match on the product name or the shop. A blank query matches everything. */
export function matchesSearch(expense: Pick<Expense, 'description' | 'merchant'>, query: string): boolean {
  const needle = query.trim().toLowerCase();
  if (!needle) return true;
  return expense.description.toLowerCase().includes(needle) || (expense.merchant ?? '').toLowerCase().includes(needle);
}
