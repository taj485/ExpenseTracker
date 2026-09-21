import { ExpenseCategory } from '../models/expense.model';
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

/** Chip order: selected categories first (latest first), then the rest in their usual order. */
export function orderCategoryChips(selected: ExpenseCategory[]): ExpenseCategory[] {
  return [...selected, ...ALL_CATEGORIES.filter(c => !selected.includes(c))];
}
