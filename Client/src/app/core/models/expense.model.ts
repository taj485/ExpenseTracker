export type ExpenseCategory = 'Food' | 'Transport' | 'Utilities' | 'Entertainment' | 'Health';

export interface Expense {
  id: number;
  amount: number;
  currency: string;
  description: string;
  category: ExpenseCategory;
  date: string;
  merchant: string | null;
  receiptId: number | null;
}

export interface AddExpenseCommand {
  amount: number;
  category: ExpenseCategory;
  description: string;
  date: string;
  merchant: string | null;
}

export interface UpdateExpenseCommand {
  amount: number;
  category: ExpenseCategory;
  description: string;
  merchant: string | null;
}

export interface CategoryStat {
  category: ExpenseCategory;
  total: number;
  count: number;
  percentage: number;
}

export interface ExtractedExpense {
  amount: number;
  category: ExpenseCategory;
  description: string;
  date: string;
  quantity: number;
  merchant: string | null;
}

export interface BatchItemError {
  index: number;
  errors: string[];
}

export interface AddExpensesBatchResult {
  addedIds: number[];
  errors: BatchItemError[];
}
