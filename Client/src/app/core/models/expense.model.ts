export type ExpenseCategory = 'Food' | 'Transport' | 'Utilities' | 'Entertainment' | 'Health';

export interface Expense {
  id: number;
  unitPrice: number;
  quantity: number;
  currency: string;
  description: string;
  category: ExpenseCategory;
  date: string;
  merchantId: number | null;
  merchant: string | null;
  merchantWebsite: string | null;
  receiptId: number | null;
}

export interface AddExpenseCommand {
  expenseTableId: number;
  unitPrice: number;
  quantity: number;
  category: ExpenseCategory;
  description: string;
  date: string;
  merchant: string | null;
}

export interface UpdateExpenseCommand {
  unitPrice: number;
  quantity: number;
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

/** One slice of the merchant donut. `isOther` marks the rolled-up tail. */
export interface MerchantStat {
  merchant: string;
  website: string | null;
  total: number;
  count: number;
  percentage: number;
  isOther: boolean;
}

export interface ExtractedExpense {
  unitPrice: number;
  category: ExpenseCategory;
  description: string;
  date: string;
  quantity: number;
  merchant: string | null;
  merchantWebsite: string | null;
}

export interface BatchItemError {
  index: number;
  errors: string[];
}

export interface AddExpensesBatchResult {
  addedIds: number[];
  errors: BatchItemError[];
}
