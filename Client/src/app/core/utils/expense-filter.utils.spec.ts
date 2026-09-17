import { orderCategoryChips, parseCategoryParam, toggleCategory } from './expense-filter.utils';

describe('parseCategoryParam', () => {
  it('returns no categories for a missing or empty param', () => {
    expect(parseCategoryParam(null)).toEqual([]);
    expect(parseCategoryParam('')).toEqual([]);
  });

  it('parses a single category, so older single-category links keep working', () => {
    expect(parseCategoryParam('Food')).toEqual(['Food']);
  });

  it('keeps the order given in the URL', () => {
    expect(parseCategoryParam('Health,Food')).toEqual(['Health', 'Food']);
  });

  it('drops unknown values, duplicates and surrounding spaces', () => {
    expect(parseCategoryParam(' Food ,Cars,Health,Food')).toEqual(['Food', 'Health']);
  });
});

describe('toggleCategory', () => {
  it('adds a newly selected category to the front', () => {
    expect(toggleCategory(['Food'], 'Health')).toEqual(['Health', 'Food']);
  });

  it('removes an already selected category', () => {
    expect(toggleCategory(['Health', 'Food'], 'Health')).toEqual(['Food']);
  });
});

describe('orderCategoryChips', () => {
  it('lists every category in the usual order when none are selected', () => {
    expect(orderCategoryChips([])).toEqual(['Food', 'Transport', 'Utilities', 'Entertainment', 'Health']);
  });

  it('puts selected categories first, then the rest in their usual order', () => {
    expect(orderCategoryChips(['Health', 'Transport'])).toEqual(['Health', 'Transport', 'Food', 'Utilities', 'Entertainment']);
  });
});
