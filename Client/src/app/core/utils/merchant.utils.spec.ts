import { merchantInitials, merchantInitialsColor, merchantLogoPaths } from './merchant.utils';

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

  describe('with a website from the merchant reference table', () => {
    it('uses the stored domain instead of guessing', () => {
      expect(merchantLogoPaths('Tesco', 'tesco.com')).toEqual(['tesco.com', 'name/Tesco']);
    });

    it('prefers the stored domain even when it differs from the guess', () => {
      expect(merchantLogoPaths('B&Q', 'diy.com')).toEqual(['diy.com', 'name/B%26Q']);
      expect(merchantLogoPaths('Marks & Spencer', 'marksandspencer.com')[0])
        .toBe('marksandspencer.com');
    });

    it('trims the stored domain', () => {
      expect(merchantLogoPaths('Tesco', '  tesco.com  ')[0]).toBe('tesco.com');
    });

    it('falls back to guessing when the merchant has no website', () => {
      expect(merchantLogoPaths('Greggs', null)).toEqual([
        'greggs.com', 'greggs.co.uk', 'name/Greggs',
      ]);
      expect(merchantLogoPaths('Greggs', '')).toEqual([
        'greggs.com', 'greggs.co.uk', 'name/Greggs',
      ]);
      expect(merchantLogoPaths('Greggs', '   ')).toEqual([
        'greggs.com', 'greggs.co.uk', 'name/Greggs',
      ]);
    });

    it('still returns nothing when there is no merchant, website or not', () => {
      expect(merchantLogoPaths(null, 'tesco.com')).toEqual([]);
      expect(merchantLogoPaths('  ', 'tesco.com')).toEqual([]);
    });
  });
});

describe('merchantInitials', () => {
  it('uses one letter for a single-word merchant', () => {
    expect(merchantInitials('Tesco')).toBe('T');
  });

  it('uses the first letter of the first two words', () => {
    expect(merchantInitials('Uber Eats')).toBe('UE');
    expect(merchantInitials('Pizza Express')).toBe('PE');
  });

  it('stops at two letters', () => {
    expect(merchantInitials('Transport for London')).toBe('TF');
  });

  it('treats an ampersand as a word break', () => {
    expect(merchantInitials('M&S')).toBe('MS');
    expect(merchantInitials('B&Q')).toBe('BQ');
    expect(merchantInitials('Marks & Spencer')).toBe('MS');
  });

  it('keeps apostrophes inside a word', () => {
    expect(merchantInitials("Sainsbury's")).toBe('S');
    expect(merchantInitials("McDonald's")).toBe('M');
    expect(merchantInitials("Papa John's")).toBe('PJ');
  });

  it('uppercases the result', () => {
    expect(merchantInitials('easyJet')).toBe('E');
    expect(merchantInitials('ao.com')).toBe('AC');
  });

  it('ignores punctuation-only segments', () => {
    expect(merchantInitials('YO! Sushi')).toBe('YS');
    expect(merchantInitials('  Costa   Coffee  ')).toBe('CC');
  });

  it('handles digits in the name', () => {
    expect(merchantInitials('Jet2')).toBe('J');
    expect(merchantInitials('7 Eleven')).toBe('7E');
  });

  it('returns empty when there is nothing to abbreviate', () => {
    expect(merchantInitials(null)).toBe('');
    expect(merchantInitials('')).toBe('');
    expect(merchantInitials('   ')).toBe('');
    expect(merchantInitials('&&&')).toBe('');
  });
});

describe('merchantInitialsColor', () => {
  it('is stable for the same merchant', () => {
    expect(merchantInitialsColor('Tesco')).toBe(merchantInitialsColor('Tesco'));
  });

  it('ignores casing and surrounding whitespace', () => {
    expect(merchantInitialsColor('  TESCO ')).toBe(merchantInitialsColor('tesco'));
  });

  it('always returns a colour, even with no merchant', () => {
    expect(merchantInitialsColor(null)).toMatch(/^#[0-9a-f]{6}$/i);
    expect(merchantInitialsColor('Greggs')).toMatch(/^#[0-9a-f]{6}$/i);
  });

  it('distinguishes at least some different merchants', () => {
    const colors = new Set(
      ['Tesco', 'Asda', 'Greggs', 'Boots', 'Shell', 'Uber'].map(merchantInitialsColor),
    );
    expect(colors.size).toBeGreaterThan(1);
  });
});
