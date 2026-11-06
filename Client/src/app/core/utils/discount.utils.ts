import { expenseTotal } from './expense.utils';

/** A receipt line that may carry a discount. An empty (null/undefined) discount means £0.00. */
export interface DiscountableLine {
  unitPrice: number;
  quantity: number;
  discount?: number | null;
}

/** A saved expense line, which also carries its receipt's discount. */
export interface PaidLine extends DiscountableLine {
  receiptId: number | null;
  receiptDiscount?: number | null;
}

/** How a receipt total is worked out: Total = Subtotal − Item discounts − Receipt discount. */
export interface ReceiptBreakdown {
  subtotal: number;
  itemDiscounts: number;
  receiptDiscount: number;
  total: number;
}

const roundPence = (amount: number): number => Math.round(amount * 100) / 100;

/** "£3.00", the way discount validation messages show amounts. */
export function formatPounds(amount: number): string {
  return `£${amount.toFixed(2)}`;
}

/** Line total after its own discount. */
export function lineTotalAfterDiscount(line: DiscountableLine): number {
  return roundPence(expenseTotal(line) - (line.discount ?? 0));
}

export function receiptBreakdown(lines: DiscountableLine[], receiptDiscount: number | null | undefined): ReceiptBreakdown {
  const subtotal = roundPence(lines.reduce((sum, l) => sum + expenseTotal(l), 0));
  const itemDiscounts = roundPence(lines.reduce((sum, l) => sum + (l.discount ?? 0), 0));
  const receipt = receiptDiscount ?? 0;
  return { subtotal, itemDiscounts, receiptDiscount: receipt, total: roundPence(subtotal - itemDiscounts - receipt) };
}

/**
 * What was actually paid for these expenses: every line after its discount, less each receipt's
 * discount once (it is repeated on every line of the receipt).
 */
export function paidTotal(lines: PaidLine[]): number {
  const receiptDiscounts = new Map<number, number>();
  let total = 0;
  for (const line of lines) {
    total += lineTotalAfterDiscount(line);
    if (line.receiptId != null) receiptDiscounts.set(line.receiptId, line.receiptDiscount ?? 0);
  }
  for (const discount of receiptDiscounts.values()) total -= discount;
  return roundPence(total);
}

/** Validation message for an item discount, or null when it is fine. */
export function itemDiscountError(line: DiscountableLine): string | null {
  const discount = line.discount ?? 0;
  if (discount < 0) return "Discount can't be negative.";
  const lineTotal = expenseTotal(line);
  if (discount > lineTotal) return `Discount can't be more than the item total (${formatPounds(lineTotal)}).`;
  return null;
}

/** Validation message for the receipt discount, or null when it is fine. */
export function receiptDiscountError(lines: DiscountableLine[], receiptDiscount: number | null | undefined): string | null {
  const discount = receiptDiscount ?? 0;
  if (discount < 0) return "Receipt discount can't be negative.";
  const { subtotal, itemDiscounts } = receiptBreakdown(lines, 0);
  const remaining = roundPence(subtotal - itemDiscounts);
  if (discount > remaining) return `Receipt discount can't be more than the receipt total (${formatPounds(remaining)}).`;
  return null;
}
