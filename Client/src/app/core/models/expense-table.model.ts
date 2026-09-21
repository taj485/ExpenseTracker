export interface ExpenseTable {
  id: number;
  name: string;
  dateCreated: string;
  isCurrentUserAdmin: boolean;
  isStarred: boolean;
  memberCount: number;
}

/** GET /api/expensetable/{id}/members — admins first, then by email. */
export interface ExpenseTableMember {
  userId: number;
  email: string | null;
  isAdmin: boolean;
  isCurrentUser: boolean;
}

export interface CreateExpenseTableCommand {
  name: string;
}

export interface InviteUserToTableCommand {
  expenseTableId: number;
  inviteeEmail: string;
  isAdmin: boolean;
}
