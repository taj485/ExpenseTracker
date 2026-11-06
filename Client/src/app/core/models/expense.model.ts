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
  /** Discount on this line in £; 0 when there is none. */
  discount?: number;
  /** Discount on the whole receipt in £, repeated on each of its lines; 0 when there is none. */
  receiptDiscount?: number;
  /** Email of whoever added it; null for expenses added before uploaders were recorded. */
  createdByEmail: string | null;
  /** True when the signed-in user added it. */
  createdByCurrentUser: boolean;
}

export interface AddExpenseCommand {
  expenseTableId: number;
  unitPrice: number;
  quantity: number;
  category: ExpenseCategory;
  description: string;
  date: string;
  merchant: string | null;
  /** Discount on this line in £. Only sent for receipt items. */
  discount?: number;
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
  /** Discount the scan found on this line in £; 0 when none. */
  discount?: number;
  /** Discount the scan found on the whole receipt in £, repeated on every item; 0 when none. */
  receiptDiscount?: number;
}

export interface BatchItemError {
  index: number;
  errors: string[];
}

export interface AddExpensesBatchResult {
  addedIds: number[];
  errors: BatchItemError[];
}
