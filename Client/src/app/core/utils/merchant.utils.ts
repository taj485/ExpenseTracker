// Merchants whose logo domain isn't <name>.com / <name>.co.uk, or that logo.dev's /name/ search
// resolves to the wrong brand. Keyed by lowercased, trimmed merchant name.
const DOMAIN_OVERRIDES: Record<string, string> = {
  'm&s':             'marksandspencer.com',  // /name/ search returns an unrelated "CPG" logo
  'marks & spencer': 'marksandspencer.com',
  'b&q':             'diy.com',
};

const COMBINING_MARKS = /[\u0300-\u036f]/g;
const NON_DOMAIN_CHARS = /[^a-z0-9]/g;

/**
 * img.logo.dev path segments to try for a merchant, best guess first.
 * Exact domains come before the fuzzy /name/ search, which can match the wrong brand.
 * Empty when there is nothing to look up.
 */
export function merchantLogoPaths(merchant: string | null): string[] {
  const name = (merchant ?? '').trim();
  if (!name) return [];

  const key = name.toLowerCase();
  const override = DOMAIN_OVERRIDES[key];

  const slug = key
    .normalize('NFD')
    .replace(COMBINING_MARKS, '')   // strip accents
    .replace(NON_DOMAIN_CHARS, ''); // drop spaces, &, apostrophes, punctuation

  const domains = override ? [override] : slug ? [`${slug}.com`, `${slug}.co.uk`] : [];

  return [...domains, `name/${encodeURIComponent(name)}`];
}
