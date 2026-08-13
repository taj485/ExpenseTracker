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
  expenseTableId: number;
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

export type ExtractionStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed';

/** Response of POST .../extract-receipt — the work is queued, not done. */
export interface StartExtractionResult {
  jobId: string;
  tempReference: string;
}

/** Shape shared by the SignalR push and the fallback poll, so both are interchangeable. */
export interface ReceiptExtractionStatus {
  jobId: string;
  status: ExtractionStatus;
  items: ExtractedExpense[];
  error: string | null;
}

export interface ExtractionOutcome {
  items: ExtractedExpense[];
  tempReference: string;
}

export interface BatchItemError {
  index: number;
  errors: string[];
}

export interface AddExpensesBatchResult {
  addedIds: number[];
  errors: BatchItemError[];
}
