const HEIC_MIME_TYPES = ['image/heic', 'image/heif'];
const HEIC_EXTENSIONS = ['.heic', '.heif'];

export function isHeicFile(file: File): boolean {
  if (HEIC_MIME_TYPES.includes(file.type.toLowerCase())) return true;
  const name = file.name.toLowerCase();
  return HEIC_EXTENSIONS.some(ext => name.endsWith(ext));
}

export async function convertIfHeic(file: File): Promise<File> {
  if (!isHeicFile(file)) return file;

  const heic2any = (await import('heic2any')).default;
  const converted = await heic2any({ blob: file, toType: 'image/jpeg', quality: 0.9 });
  const blob = Array.isArray(converted) ? converted[0] : converted;

  const newName = file.name.replace(/\.(heic|heif)$/i, '.jpg');
  return new File([blob], newName, { type: 'image/jpeg' });
}
