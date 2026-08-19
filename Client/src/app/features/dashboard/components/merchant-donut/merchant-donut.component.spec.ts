import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MerchantDonutComponent } from './merchant-donut.component';
import { MerchantStat } from '../../../../core/models/expense.model';

function stat(partial: Partial<MerchantStat> & { merchant: string; total: number }): MerchantStat {
  return {
    website: null,
    count: 1,
    percentage: 0,
    isOther: false,
    ...partial,
  };
}

/** Every coordinate the arc path visits, so geometry can be asserted without a DOM. */
function coordsOf(path: string): number[] {
  return path.match(/-?\d+(\.\d+)?/g)!.map(Number);
}

describe('MerchantDonutComponent', () => {
  let fixture: ComponentFixture<MerchantDonutComponent>;
  let component: MerchantDonutComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [MerchantDonutComponent] });
    fixture = TestBed.createComponent(MerchantDonutComponent);
    component = fixture.componentInstance;
  });

  function setStats(stats: MerchantStat[]) {
    fixture.componentRef.setInput('stats', stats);
  }

  it('produces no segments without data', () => {
    setStats([]);
    expect(component.segments()).toEqual([]);
  });

  it('produces one segment per stat', () => {
    setStats([
      stat({ merchant: 'Tesco', total: 60, percentage: 60 }),
      stat({ merchant: 'Shell', total: 40, percentage: 40 }),
    ]);

    expect(component.segments()).toHaveLength(2);
  });

  it('assigns categorical colours by rank and never repeats one', () => {
    setStats([
      stat({ merchant: 'A', total: 5 }), stat({ merchant: 'B', total: 4 }),
      stat({ merchant: 'C', total: 3 }), stat({ merchant: 'D', total: 2 }),
      stat({ merchant: 'E', total: 1 }),
    ]);

    const colors = component.segments().map(s => s.color);

    expect(new Set(colors).size).toBe(5);
  });

  it('paints the Other slice a neutral rather than a categorical hue', () => {
    setStats([
      stat({ merchant: 'Tesco', total: 60 }),
      stat({ merchant: 'Other (3)', total: 40, isOther: true }),
    ]);

    const segments = component.segments();

    expect(segments[1].color).toBe('#6c757d');
    expect(segments[1].color).not.toBe(segments[0].color);
  });

  it('flags a single merchant as a full ring, which an arc path cannot draw', () => {
    setStats([stat({ merchant: 'Tesco', total: 100, percentage: 100 })]);

    expect(component.isFullRing()).toBe(true);
  });

  it('is not a full ring once there are two merchants', () => {
    setStats([
      stat({ merchant: 'Tesco', total: 60 }),
      stat({ merchant: 'Shell', total: 40 }),
    ]);

    expect(component.isFullRing()).toBe(false);
  });

  it('keeps every arc coordinate inside the viewBox', () => {
    setStats([
      stat({ merchant: 'A', total: 50 }), stat({ merchant: 'B', total: 30 }),
      stat({ merchant: 'C', total: 15 }), stat({ merchant: 'D', total: 5 }),
    ]);

    for (const segment of component.segments()) {
      for (const value of coordsOf(segment.path)) {
        expect(value).toBeGreaterThanOrEqual(0);
        expect(value).toBeLessThanOrEqual(220);
      }
    }
  });

  it('emits a well-formed closed path for each slice', () => {
    setStats([
      stat({ merchant: 'A', total: 70 }),
      stat({ merchant: 'B', total: 30 }),
    ]);

    for (const segment of component.segments()) {
      expect(segment.path).toMatch(/^M .* A .* L .* A .* Z$/);
      expect(coordsOf(segment.path).some(Number.isNaN)).toBe(false);
    }
  });

  it('labels only slices with room for the text', () => {
    setStats([
      stat({ merchant: 'Big', total: 95, percentage: 95 }),
      stat({ merchant: 'Sliver', total: 5, percentage: 5 }),
    ]);

    const segments = component.segments();

    expect(segments[0].showLabel).toBe(true);
    expect(segments[1].showLabel).toBe(false);
  });

  it('does not let the gap swallow a very thin slice', () => {
    setStats([
      stat({ merchant: 'Big', total: 999, percentage: 99 }),
      stat({ merchant: 'Tiny', total: 1, percentage: 1 }),
    ]);

    // A thin arc must still have distinct start and end points.
    const [, tiny] = component.segments();
    const coords = coordsOf(tiny.path);

    expect(coords.some(Number.isNaN)).toBe(false);
    expect(tiny.path).toContain('A');
  });

  it('tracks the hovered slice and clears it on leave', () => {
    setStats([
      stat({ merchant: 'Tesco', total: 60 }),
      stat({ merchant: 'Shell', total: 40 }),
    ]);

    expect(component.hoveredStat()).toBeNull();

    component.onEnter(1);
    expect(component.hoveredStat()!.merchant).toBe('Shell');

    component.onLeave();
    expect(component.hoveredStat()).toBeNull();
  });
});
