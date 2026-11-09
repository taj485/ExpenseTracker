import {
  formatQuantity,
  formatQuantityLine,
  formatUnitPrice,
  quantityError,
  quantityStep,
  showsQuantityLine,
  unitLabel,
} from './quantity.utils';

describe('quantity.utils', () => {
  describe('unitLabel', () => {
    it('gives the short label for each unit', () => {
      expect(unitLabel('Item')).toBe('each');
      expect(unitLabel('Kg')).toBe('kg');
      expect(unitLabel('G')).toBe('g');
      expect(unitLabel('Lb')).toBe('lb');
      expect(unitLabel('L')).toBe('L');
    });
  });

  describe('quantityStep', () => {
    it('steps whole numbers for items and thousandths for measured units', () => {
      expect(quantityStep('Item')).toBe('1');
      expect(quantityStep('Kg')).toBe('0.001');
    });
  });

  describe('quantityError', () => {
    it('accepts whole items of at least 1', () => {
      expect(quantityError(1, 'Item')).toBeNull();
      expect(quantityError(3, 'Item')).toBeNull();
    });

    it('rejects fractional, zero or missing item quantities', () => {
      expect(quantityError(1.5, 'Item')).toBe('Quantity must be a whole number of at least 1.');
      expect(quantityError(0, 'Item')).toBe('Quantity must be a whole number of at least 1.');
      expect(quantityError(NaN, 'Item')).toBe('Quantity is required.');
    });

    it('accepts fractional weights to 3 decimal places', () => {
      expect(quantityError(0.612, 'Kg')).toBeNull();
      expect(quantityError(0.001, 'Kg')).toBeNull();
      expect(quantityError(2, 'L')).toBeNull();
    });

    it('rejects weights of 0 or less, or with more than 3 decimal places', () => {
      expect(quantityError(0, 'Kg')).toBe('Quantity must be greater than 0.');
      expect(quantityError(-0.5, 'Kg')).toBe('Quantity must be greater than 0.');
      expect(quantityError(0.6125, 'Kg')).toBe('Quantity can have at most 3 decimal places.');
    });
  });

  describe('formatting', () => {
    it('shows the unit for measured quantities only', () => {
      expect(formatQuantity(0.612, 'Kg')).toBe('0.612 kg');
      expect(formatQuantity(2, 'Item')).toBe('2');
    });

    it('shows a per-unit price for measured quantities only', () => {
      expect(formatUnitPrice(5, 'Kg')).toBe('£5.00/kg');
      expect(formatUnitPrice(1.2, 'Item')).toBe('£1.20');
    });

    it('builds the quantity × price line', () => {
      expect(formatQuantityLine({ quantity: 0.612, unit: 'Kg', unitPrice: 5 })).toBe('0.612 kg × £5.00/kg');
      expect(formatQuantityLine({ quantity: 2, unit: 'Item', unitPrice: 1.2 })).toBe('2 × £1.20');
    });
  });

  describe('showsQuantityLine', () => {
    it('hides the line for a single item but shows it for several items or any weight', () => {
      expect(showsQuantityLine({ quantity: 1, unit: 'Item' })).toBe(false);
      expect(showsQuantityLine({ quantity: 2, unit: 'Item' })).toBe(true);
      expect(showsQuantityLine({ quantity: 1, unit: 'Kg' })).toBe(true);
    });
  });
});
