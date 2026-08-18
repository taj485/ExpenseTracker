/** Line total for an expense: unit price x quantity, rounded to whole pence. */
export function expenseTotal(expense: { unitPrice: number; quantity: number }): number {
  return Math.round(expense.unitPrice * expense.quantity * 100) / 100;
}
