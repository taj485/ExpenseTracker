export interface ExpenseTable {
  id: number;
  name: string;
  dateCreated: string;
  isCurrentUserAdmin: boolean;
  memberCount: number;
}

export interface CreateExpenseTableCommand {
  name: string;
}
