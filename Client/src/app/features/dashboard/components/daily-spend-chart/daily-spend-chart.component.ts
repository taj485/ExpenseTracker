import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { DailySpend } from '../../../../core/utils/expense.utils';
import { formatDayLabel } from '../../../../core/utils/date.utils';

/** Rounds up to 1, 2 or 5 × a power of ten, so the axis tops out on a readable figure. */
export function niceCeiling(value: number): number {
  if (value <= 0) return 1;
  const magnitude = 10 ** Math.floor(Math.log10(value));
  const step = [1, 2, 5, 10].find(s => value <= s * magnitude)!;
  return step * magnitude;
}

/**
 * One bar per day of the month. A single series, so one brand colour and no legend; the section
 * title names it. The tallest day carries its value as a direct label, every bar has a hover
 * tooltip, and a visually hidden table gives screen readers the same numbers.
 */
@Component({
  selector: 'app-daily-spend-chart',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './daily-spend-chart.component.html',
  styleUrl: './daily-spend-chart.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DailySpendChartComponent {
  readonly days = input.required<DailySpend[]>();
  readonly monthLabel = input('');

  /** Index of the day under the pointer, for the tooltip. */
  readonly hovered = signal<number | null>(null);

  readonly hasSpend = computed(() => this.days().some(d => d.total > 0));

  readonly scaleMax = computed(() => niceCeiling(Math.max(...this.days().map(d => d.total), 0)));

  /** Gridlines at zero, halfway and the top, as a share of the plot height. */
  readonly ticks = computed(() => {
    const max = this.scaleMax();
    return [0, max / 2, max].map(value => ({ value, percent: (value / max) * 100 }));
  });

  /** The single direct label goes on the tallest day (the first, if days tie). */
  readonly peakIndex = computed(() => {
    const days = this.days();
    return days.reduce((best, d, i) => (d.total > days[best].total ? i : best), 0);
  });

  /** Days with spending, for the screen-reader table. */
  readonly spendDays = computed(() => this.days().filter(d => d.total > 0));

  readonly tooltip = computed(() => {
    const index = this.hovered();
    const day = index === null ? undefined : this.days()[index];
    if (!day || day.isFuture) return null;
    const count = this.days().length;
    // Centred over its day, except near the ends, where it hangs inward instead of off the chart.
    const align = index! < 3 ? 'start' : index! >= count - 3 ? 'end' : 'center';
    return { day, label: formatDayLabel(day.date), left: ((index! + 0.5) / count) * 100, align };
  });

  barHeight(total: number): number {
    return (total / this.scaleMax()) * 100;
  }

  /** Day numbers under the axis: the 1st and every 5th, so 31 labels never crowd. */
  showDayLabel(day: number): boolean {
    return day === 1 || day % 5 === 0;
  }

  dayLabel(date: string): string {
    return formatDayLabel(date);
  }
}
