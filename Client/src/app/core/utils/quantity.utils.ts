import { QuantityUnit } from '../models/expense.model';

/** Units offered in the forms, with the short label shown after a quantity. */
export const QUANTITY_UNITS: readonly { value: QuantityUnit; label: string }[] = [
  { value: 'Item', label: 'each' },
  { value: 'Kg', label: 'kg' },
  { value: 'G', label: 'g' },
  { value: 'Lb', label: 'lb' },
  { value: 'L', label: 'L' },
];

export function unitLabel(unit: QuantityUnit): string {
  return QUANTITY_UNITS.find(u => u.value === unit)?.label ?? '';
}

export function isMeasured(unit: QuantityUnit): boolean {
  return unit !== 'Item';
}

/** Step for the quantity input: whole items, or grams-level precision for weights and volumes. */
export function quantityStep(unit: QuantityUnit): string {
  return isMeasured(unit) ? '0.001' : '1';
}

/** Why a quantity is not allowed for its unit, or null when it is. Mirrors the API's rule. */
export function quantityError(quantity: number, unit: QuantityUnit): string | null {
  if (isNaN(quantity)) return 'Quantity is required.';
  if (!isMeasured(unit)) {
    return Number.isInteger(quantity) && quantity >= 1 ? null : 'Quantity must be a whole number of at least 1.';
  }
  if (quantity <= 0) return 'Quantity must be greater than 0.';
  if (Math.round(quantity * 1000) / 1000 !== quantity) return 'Quantity can have at most 3 decimal places.';
  return null;
}

/** "0.612 kg" for measured amounts; just "2" for items. */
export function formatQuantity(quantity: number, unit: QuantityUnit): string {
  return isMeasured(unit) ? `${quantity} ${unitLabel(unit)}` : `${quantity}`;
}

/** "£5.00/kg" for measured amounts; "£1.20" for items. */
export function formatUnitPrice(unitPrice: number, unit: QuantityUnit): string {
  const price = `£${unitPrice.toFixed(2)}`;
  return isMeasured(unit) ? `${price}/${unitLabel(unit)}` : price;
}

/** "0.612 kg × £5.00/kg" or "2 × £1.20". */
export function formatQuantityLine(item: { quantity: number; unit: QuantityUnit; unitPrice: number }): string {
  return `${formatQuantity(item.quantity, item.unit)} × ${formatUnitPrice(item.unitPrice, item.unit)}`;
}

/** Whether the "quantity × price" line adds anything beyond the total (a single item does not). */
export function showsQuantityLine(item: { quantity: number; unit: QuantityUnit }): boolean {
  return isMeasured(item.unit) || item.quantity !== 1;
}
