import { Injectable } from '@angular/core';

export interface ResizeOptions {
  maxWidth?: number;
  maxHeight?: number;
  quality?: number;
}

const DEFAULT_MAX_WIDTH = 1600;
const DEFAULT_MAX_HEIGHT = 1600;
const DEFAULT_QUALITY = 0.8;

@Injectable({ providedIn: 'root' })
export class ImageResizeService {
  async resizeImage(file: File | Blob, options?: ResizeOptions): Promise<Blob> {
    const maxWidth = options?.maxWidth ?? DEFAULT_MAX_WIDTH;
    const maxHeight = options?.maxHeight ?? DEFAULT_MAX_HEIGHT;
    const quality = options?.quality ?? DEFAULT_QUALITY;

    const bitmap = await createImageBitmap(file);
    try {
      const { width, height } = this.calculateDimensions(bitmap.width, bitmap.height, maxWidth, maxHeight);

      const canvas = document.createElement('canvas');
      canvas.width = width;
      canvas.height = height;

      const ctx = canvas.getContext('2d');
      if (!ctx) throw new Error('Could not create a canvas context for image resizing.');

      ctx.imageSmoothingQuality = 'high';
      ctx.drawImage(bitmap, 0, 0, width, height);

      return await new Promise<Blob>((resolve, reject) => {
        canvas.toBlob(
          blob => (blob ? resolve(blob) : reject(new Error('Failed to encode resized image.'))),
          'image/jpeg',
          quality,
        );
      });
    } finally {
      bitmap.close();
    }
  }

  calculateDimensions(
    sourceWidth: number,
    sourceHeight: number,
    maxWidth: number,
    maxHeight: number,
  ): { width: number; height: number } {
    if (sourceWidth <= maxWidth && sourceHeight <= maxHeight) {
      return { width: sourceWidth, height: sourceHeight };
    }

    const scale = Math.min(maxWidth / sourceWidth, maxHeight / sourceHeight);
    return {
      width: Math.round(sourceWidth * scale),
      height: Math.round(sourceHeight * scale),
    };
  }
}
