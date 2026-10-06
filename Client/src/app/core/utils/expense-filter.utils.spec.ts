import { matchesSearch, orderCategoryChips, parseCategoryParam, parseMonthParam, toggleCategory, toggleMonth } from './expense-filter.utils';

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

describe('parseMonthParam', () => {
  it('returns no months for a missing or empty param', () => {
    expect(parseMonthParam(null)).toEqual([]);
    expect(parseMonthParam('')).toEqual([]);
  });

  it('parses a single month, so older single-month links keep working', () => {
    expect(parseMonthParam('2026-09')).toEqual(['2026-09']);
  });

  it('sorts newest first and drops malformed values, duplicates and spaces', () => {
    expect(parseMonthParam(' 2026-08,2026-10,nope,2026-8,2026-10 ')).toEqual(['2026-10', '2026-08']);
  });
});

describe('toggleMonth', () => {
  it('adds a month in date order', () => {
    expect(toggleMonth(['2026-10', '2026-07'], '2026-09')).toEqual(['2026-10', '2026-09', '2026-07']);
  });

  it('removes a month that is already selected', () => {
    expect(toggleMonth(['2026-10', '2026-09'], '2026-10')).toEqual(['2026-09']);
  });
});

describe('matchesSearch', () => {
  const chocolate = { description: 'Dark Chocolate', merchant: 'Asda' };

  it('matches part of the product name, ignoring case and surrounding spaces', () => {
    expect(matchesSearch(chocolate, '  CHOC ')).toBe(true);
  });

  it('matches the shop', () => {
    expect(matchesSearch(chocolate, 'asda')).toBe(true);
  });

  it('rejects text in neither', () => {
    expect(matchesSearch(chocolate, 'milk')).toBe(false);
  });

  it('matches everything for a blank query, and copes with no shop', () => {
    expect(matchesSearch(chocolate, '   ')).toBe(true);
    expect(matchesSearch({ description: 'Bus fare', merchant: null }, 'tesco')).toBe(false);
  });
});
