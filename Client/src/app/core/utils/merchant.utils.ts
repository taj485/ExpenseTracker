// Merchants whose logo domain isn't <name>.com / <name>.co.uk, or that logo.dev's /name/ search
// resolves to the wrong brand. Keyed by lowercased, trimmed merchant name.
//
// Only reached for merchants with no website on their reference-table row — i.e. ones a user
// created by saving an unrecognised receipt. Seeded merchants carry their real domain.
const DOMAIN_OVERRIDES: Record<string, string> = {
  'm&s':             'marksandspencer.com',  // /name/ search returns an unrelated "CPG" logo
  'marks & spencer': 'marksandspencer.com',
  'b&q':             'diy.com',
};

const COMBINING_MARKS = /[\u0300-\u036f]/g;
const NON_DOMAIN_CHARS = /[^a-z0-9]/g;

/**
 * img.logo.dev path segments to try for a merchant, best guess first.
 * The website recorded against the merchant wins outright; otherwise we fall back to guessing
 * domains, then to the fuzzy /name/ search, which can match the wrong brand.
 * Empty when there is nothing to look up.
 */
export function merchantLogoPaths(merchant: string | null, website?: string | null): string[] {
  const name = (merchant ?? '').trim();
  if (!name) return [];

  const knownDomain = (website ?? '').trim();
  if (knownDomain) return [knownDomain, `name/${encodeURIComponent(name)}`];

  const key = name.toLowerCase();
  const override = DOMAIN_OVERRIDES[key];

  const slug = key
    .normalize('NFD')
    .replace(COMBINING_MARKS, '')   // strip accents
    .replace(NON_DOMAIN_CHARS, ''); // drop spaces, &, apostrophes, punctuation

  const domains = override ? [override] : slug ? [`${slug}.com`, `${slug}.co.uk`] : [];

  return [...domains, `name/${encodeURIComponent(name)}`];
}

// Apostrophes stay inside a word so "Sainsbury's" is one word, not two.
// Everything else — spaces, &, dots, hyphens — separates: "M&S" gives M and S.
const WORD_SEPARATORS = /[^\p{L}\p{N}'’]+/u;
const APOSTROPHES = /['’]/g;

/** Monogram shown when no logo image could be loaded. At most two letters. */
export function merchantInitials(merchant: string | null): string {
  const words = (merchant ?? '')
    .trim()
    .split(WORD_SEPARATORS)
    .map(word => word.replace(APOSTROPHES, ''))
    .filter(word => word.length > 0);

  return words
    .slice(0, 2)
    .map(word => word[0])
    .join('')
    .toUpperCase();
}

// Chosen to stay legible under white text; deliberately not the category palette,
// which carries its own meaning.
const INITIALS_COLORS = [
  '#1565c0', '#2e7d32', '#c62828', '#6a1b9a', '#ef6c00',
  '#00838f', '#4e342e', '#ad1457', '#37474f', '#558b2f',
];

/** Stable colour per merchant, so the same shop always gets the same badge. */
export function merchantInitialsColor(merchant: string | null): string {
  const name = (merchant ?? '').trim().toLowerCase();
  if (!name) return INITIALS_COLORS[0];

  let hash = 0;
  for (let i = 0; i < name.length; i++) {
    hash = (hash * 31 + name.charCodeAt(i)) | 0;
  }

  return INITIALS_COLORS[Math.abs(hash) % INITIALS_COLORS.length];
}
