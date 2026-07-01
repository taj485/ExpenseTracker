import { ExpenseCategory } from '../models/expense.model';

export interface CategoryMeta {
  icon: string;
  badgeClass: string;
  barColor: string;
  bgColor: string;
}

const CATEGORY_META: Record<ExpenseCategory, CategoryMeta> = {
  Food:          { icon: '🍔', badgeClass: 'badge-food',          barColor: '#e65100', bgColor: '#fff3e0' },
  Transport:     { icon: '🚌', badgeClass: 'badge-transport',     barColor: '#1565c0', bgColor: '#e3f2fd' },
  Utilities:     { icon: '⚡', badgeClass: 'badge-utilities',     barColor: '#2e7d32', bgColor: '#e8f5e9' },
  Entertainment: { icon: '🎬', badgeClass: 'badge-entertainment', barColor: '#6a1b9a', bgColor: '#f3e5f5' },
  Health:        { icon: '💊', badgeClass: 'badge-health',        barColor: '#880e4f', bgColor: '#fce4ec' },
};

export function getCategoryMeta(category: ExpenseCategory): CategoryMeta {
  return CATEGORY_META[category];
}

export const ALL_CATEGORIES: ExpenseCategory[] = [
  'Food', 'Transport', 'Utilities', 'Entertainment', 'Health'
];
