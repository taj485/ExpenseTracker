import { ImageResizeService } from './image-resize.service';

describe('ImageResizeService', () => {
  let service: ImageResizeService;

  beforeEach(() => {
    service = new ImageResizeService();
  });

  it('scales down a landscape image so the long edge fits the max width', () => {
    const result = service.calculateDimensions(4000, 3000, 1600, 1600);
    expect(result).toEqual({ width: 1600, height: 1200 });
  });

  it('scales down a portrait image so the long edge fits the max height', () => {
    const result = service.calculateDimensions(3000, 4000, 1600, 1600);
    expect(result).toEqual({ width: 1200, height: 1600 });
  });

  it('scales down a square image proportionally', () => {
    const result = service.calculateDimensions(2000, 2000, 1600, 1600);
    expect(result).toEqual({ width: 1600, height: 1600 });
  });

  it('does not upscale an image already within the limits', () => {
    const result = service.calculateDimensions(800, 600, 1600, 1600);
    expect(result).toEqual({ width: 800, height: 600 });
  });
});
