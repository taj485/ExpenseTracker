import { isHeicFile } from './heic-converter';

function makeFile(name: string, type: string): File {
  return new File(['x'], name, { type });
}

describe('isHeicFile', () => {
  it('detects HEIC files by MIME type', () => {
    expect(isHeicFile(makeFile('photo.heic', 'image/heic'))).toBe(true);
  });

  it('detects HEIF files by MIME type', () => {
    expect(isHeicFile(makeFile('photo.heif', 'image/heif'))).toBe(true);
  });

  it('detects HEIC files by extension when MIME type is missing', () => {
    expect(isHeicFile(makeFile('photo.HEIC', ''))).toBe(true);
  });

  it('returns false for non-HEIC files', () => {
    expect(isHeicFile(makeFile('photo.jpg', 'image/jpeg'))).toBe(false);
  });
});
