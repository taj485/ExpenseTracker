/**
 * Who added an expense, for display. The API only stores emails, so:
 *  - 'short' → "you" or the part before "@" (e.g. "emma.carter"), for compact receipt cards
 *  - 'full'  → "You" or the whole email, for the expense detail page
 * Null when the uploader wasn't recorded (expenses added before uploaders were tracked).
 */
export function uploaderLabel(
  expense: { createdByEmail?: string | null; createdByCurrentUser?: boolean },
  style: 'short' | 'full',
): string | null {
  if (expense.createdByCurrentUser === true) return style === 'short' ? 'you' : 'You';

  const email = expense.createdByEmail?.trim();
  if (!email) return null;

  return style === 'short' ? email.split('@')[0] : email;
}
