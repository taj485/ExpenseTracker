import { merchantLogoPaths } from './merchant.utils';

describe('merchantLogoPaths', () => {
  it('tries .com then .co.uk before the name search', () => {
    expect(merchantLogoPaths('Tesco')).toEqual(['tesco.com', 'tesco.co.uk', 'name/Tesco']);
  });

  it('strips spaces and punctuation from the domain guess but keeps the name intact', () => {
    expect(merchantLogoPaths('Uber Eats')).toEqual([
      'ubereats.com', 'ubereats.co.uk', 'name/Uber%20Eats',
    ]);
  });

  it('url-encodes the name search path', () => {
    expect(merchantLogoPaths("Sainsbury's")).toEqual([
      'sainsburys.com', 'sainsburys.co.uk', "name/Sainsbury's",
    ]);
  });

  it('normalises accents in the domain guess', () => {
    expect(merchantLogoPaths('Café Nero')[0]).toBe('cafenero.com');
  });

  it('trims surrounding whitespace', () => {
    expect(merchantLogoPaths('  Greggs  ')).toEqual(['greggs.com', 'greggs.co.uk', 'name/Greggs']);
  });

  it('uses an override instead of guessing domains', () => {
    expect(merchantLogoPaths('M&S')).toEqual(['marksandspencer.com', 'name/M%26S']);
  });

  it('matches overrides case-insensitively', () => {
    // The name search path keeps the original casing, so only the domain is expected to match.
    expect(merchantLogoPaths('b&q')[0]).toBe('diy.com');
    expect(merchantLogoPaths('B&Q')[0]).toBe('diy.com');
  });

  it('falls back to the name search alone when nothing is left to slugify', () => {
    expect(merchantLogoPaths('&&&')).toEqual(['name/%26%26%26']);
  });

  it('returns nothing for a missing or blank merchant', () => {
    expect(merchantLogoPaths(null)).toEqual([]);
    expect(merchantLogoPaths('')).toEqual([]);
    expect(merchantLogoPaths('   ')).toEqual([]);
  });
});
