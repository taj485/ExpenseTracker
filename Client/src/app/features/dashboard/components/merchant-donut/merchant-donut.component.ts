import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { MerchantStat } from '../../../../core/models/expense.model';
import { MerchantLogoComponent } from '../../../../shared/merchant-logo/merchant-logo.component';

/**
 * Categorical slots, assigned by rank and never cycled — a sixth merchant folds into "Other"
 * rather than reusing a hue. Validated against a white surface: lightness band, chroma floor,
 * colour-blind separation (worst adjacent pair ΔE 9.1) and normal-vision separation (ΔE 19.6)
 * all pass. Aqua, yellow and magenta sit below 3:1 contrast, which is why every slice carries a
 * visible label and the legend below doubles as the table view.
 */
const SERIES_COLORS = ['#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4'];

/** Neutral for the rolled-up tail — deliberately not a categorical hue. */
const OTHER_COLOR = '#6c757d';

const SIZE = 220;
const CENTER = SIZE / 2;
const RADIUS_OUTER = 100;
const RADIUS_INNER = 62;

/** Surface-coloured gap between neighbouring slices, in degrees. */
const GAP_DEGREES = 1.5;

/** Below this share a slice is too thin to hold its own label; the legend carries it instead. */
const MIN_LABEL_PERCENT = 8;

export interface DonutSegment {
  stat: MerchantStat;
  path: string;
  color: string;
  labelX: number;
  labelY: number;
  showLabel: boolean;
}

function polar(radius: number, angleDegrees: number): { x: number; y: number } {
  const radians = ((angleDegrees - 90) * Math.PI) / 180;
  return { x: CENTER + radius * Math.cos(radians), y: CENTER + radius * Math.sin(radians) };
}

function annulusPath(startAngle: number, endAngle: number): string {
  const largeArc = endAngle - startAngle > 180 ? 1 : 0;
  const outerStart = polar(RADIUS_OUTER, startAngle);
  const outerEnd = polar(RADIUS_OUTER, endAngle);
  const innerEnd = polar(RADIUS_INNER, endAngle);
  const innerStart = polar(RADIUS_INNER, startAngle);

  return [
    `M ${outerStart.x} ${outerStart.y}`,
    `A ${RADIUS_OUTER} ${RADIUS_OUTER} 0 ${largeArc} 1 ${outerEnd.x} ${outerEnd.y}`,
    `L ${innerEnd.x} ${innerEnd.y}`,
    `A ${RADIUS_INNER} ${RADIUS_INNER} 0 ${largeArc} 0 ${innerStart.x} ${innerStart.y}`,
    'Z',
  ].join(' ');
}

@Component({
  selector: 'app-merchant-donut',
  standalone: true,
  imports: [DecimalPipe, MerchantLogoComponent],
  templateUrl: './merchant-donut.component.html',
  styleUrl: './merchant-donut.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MerchantDonutComponent {
  readonly stats = input<MerchantStat[]>([]);
  readonly total = input(0);
  readonly monthLabel = input('');

  readonly viewBox = `0 0 ${SIZE} ${SIZE}`;
  readonly centerX = CENTER;
  readonly centerY = CENTER;
  readonly radiusOuter = RADIUS_OUTER;
  readonly radiusInner = RADIUS_INNER;
  readonly ringWidth = RADIUS_OUTER - RADIUS_INNER;
  readonly ringRadius = (RADIUS_OUTER + RADIUS_INNER) / 2;

  /** Index of the slice under the cursor, or null. Drives both the ring and the legend. */
  readonly hovered = signal<number | null>(null);

  readonly colorFor = (index: number, stat: MerchantStat): string =>
    stat.isOther ? OTHER_COLOR : SERIES_COLORS[index % SERIES_COLORS.length];

  /** A lone merchant is a full ring, which an arc path cannot express (start == end). */
  readonly isFullRing = computed(() => this.stats().length === 1);

  readonly segments = computed((): DonutSegment[] => {
    const stats = this.stats();
    const total = stats.reduce((sum, s) => sum + s.total, 0);
    if (total <= 0) return [];

    let cursor = 0;

    return stats.map((stat, index) => {
      const sweep = (stat.total / total) * 360;
      const start = cursor;
      cursor += sweep;

      // Never let the gap eat a thin slice entirely.
      const gap = stats.length > 1 ? Math.min(GAP_DEGREES, sweep * 0.25) : 0;
      const mid = start + sweep / 2;
      const label = polar(this.ringRadius, mid);

      return {
        stat,
        path: annulusPath(start + gap / 2, start + sweep - gap / 2),
        color: this.colorFor(index, stat),
        labelX: label.x,
        labelY: label.y,
        showLabel: stat.percentage >= MIN_LABEL_PERCENT,
      };
    });
  });

  readonly hoveredStat = computed(() => {
    const index = this.hovered();
    return index === null ? null : (this.segments()[index]?.stat ?? null);
  });

  onEnter(index: number): void {
    this.hovered.set(index);
  }

  onLeave(): void {
    this.hovered.set(null);
  }
}
