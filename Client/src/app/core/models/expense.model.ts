export type ExpenseCategory = 'Food' | 'Transport' | 'Utilities' | 'Entertainment' | 'Health';

export interface Expense {
  id: number;
  amount: number;
  currency: string;
  description: string;
  category: ExpenseCategory;
  date: string;
}

export interface AddExpenseCommand {
  amount: number;
  category: ExpenseCategory;
  description: string;
}

export interface CategoryStat {
  category: ExpenseCategory;
  total: number;
  count: number;
  percentage: number;
}
