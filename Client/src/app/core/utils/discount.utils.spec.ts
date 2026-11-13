import {
  addDiscounts,
  formatPounds,
  itemDiscountError,
  lineTotalAfterDiscount,
  paidTotal,
  receiptBreakdown,
  receiptDiscountError,
} from './discount.utils';

const milk = { unitPrice: 1.5, quantity: 2, discount: 0.5 };
const bread = { unitPrice: 2, quantity: 1, discount: null };

describe('discount.utils', () => {
  it('adds two discounts to the penny, treating empty as zero', () => {
    expect(addDiscounts(0.1, 0.2)).toBe(0.3);
    expect(addDiscounts(null, 2)).toBe(2);
    expect(addDiscounts(undefined, null)).toBe(0);
  });

  it('formats pounds to 2 decimal places', () => {
    expect(formatPounds(3)).toBe('£3.00');
  });

  it('takes the discount off the whole line', () => {
    expect(lineTotalAfterDiscount(milk)).toBe(2.5);
    expect(lineTotalAfterDiscount(bread)).toBe(2);
  });

  it('works out subtotal, discounts and total', () => {
    expect(receiptBreakdown([milk, bread], 1)).toEqual({ subtotal: 5, itemDiscounts: 0.5, receiptDiscount: 1, total: 3.5 });
  });

  it('counts each receipt discount once in the paid total', () => {
    const lines = [
      { ...milk, receiptId: 1, receiptDiscount: 1 },
      { ...bread, receiptId: 1, receiptDiscount: 1 },
      { unitPrice: 4, quantity: 1, receiptId: null },
    ];
    expect(paidTotal(lines)).toBe(7.5);
  });

  it('rejects negative and too-large item discounts, and allows a free item', () => {
    expect(itemDiscountError({ ...milk, discount: -0.5 })).toBe("Discount can't be negative.");
    expect(itemDiscountError({ ...milk, discount: 3.5 })).toBe("Discount can't be more than the item total (£3.00).");
    expect(itemDiscountError({ ...milk, discount: 3 })).toBeNull();
  });

  it('rejects a receipt discount larger than the total after item discounts', () => {
    expect(receiptDiscountError([milk, bread], 5)).toBe("Receipt discount can't be more than the receipt total (£4.50).");
    expect(receiptDiscountError([milk, bread], 4.5)).toBeNull();
    expect(receiptDiscountError([milk, bread], -1)).toBe("Receipt discount can't be negative.");
  });

  it('rounds every amount to the penny so floating point noise never shows', () => {
    const noisy = [{ unitPrice: 0.1, quantity: 1 }, { unitPrice: 0.2, quantity: 1 }];
    expect(lineTotalAfterDiscount({ unitPrice: 0.1, quantity: 3, discount: 0 })).toBe(0.3);
    expect(receiptBreakdown(noisy, 0).subtotal).toBe(0.3);
    expect(receiptBreakdown([{ unitPrice: 0.3, quantity: 1, discount: 0.1 }, { unitPrice: 0.3, quantity: 1, discount: 0.2 }], 0).itemDiscounts).toBe(0.3);
    expect(receiptBreakdown(noisy, 0.1).total).toBe(0.2);
    expect(paidTotal(noisy.map(l => ({ ...l, receiptId: null })))).toBe(0.3);
    expect(receiptDiscountError(noisy, 0.3)).toBeNull();
  });

  it('treats a missing receipt discount as nothing off', () => {
    expect(receiptBreakdown([bread], undefined).total).toBe(2);
    expect(receiptBreakdown([bread], null).receiptDiscount).toBe(0);
    expect(paidTotal([{ ...bread, receiptId: 1 }])).toBe(2);
    expect(receiptDiscountError([bread], null)).toBeNull();
  });
});
