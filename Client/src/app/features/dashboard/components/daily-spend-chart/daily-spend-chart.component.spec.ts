import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DailySpendChartComponent, niceCeiling } from './daily-spend-chart.component';
import { DailySpend } from '../../../../core/utils/expense.utils';

function day(n: number, total: number, isFuture = false): DailySpend {
  return { date: `2026-10-${String(n).padStart(2, '0')}`, day: n, total, isFuture };
}

describe('niceCeiling', () => {
  it('rounds up to 1, 2 or 5 times a power of ten', () => {
    expect(niceCeiling(0.4)).toBe(0.5);
    expect(niceCeiling(4.5)).toBe(5);
    expect(niceCeiling(12)).toBe(20);
    expect(niceCeiling(51.89)).toBe(100);
    expect(niceCeiling(200)).toBe(200);
  });

  it('never returns zero, so bar heights can always be divided by it', () => {
    expect(niceCeiling(0)).toBe(1);
  });
});

describe('DailySpendChartComponent', () => {
  let fixture: ComponentFixture<DailySpendChartComponent>;
  let component: DailySpendChartComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [DailySpendChartComponent] });
    fixture = TestBed.createComponent(DailySpendChartComponent);
    component = fixture.componentInstance;
  });

  function setDays(days: DailySpend[]) {
    fixture.componentRef.setInput('days', days);
    fixture.componentRef.setInput('monthLabel', 'October 2026');
    fixture.detectChanges();
  }

  it('scales bars against a rounded axis top', () => {
    setDays([day(1, 2), day(2, 0), day(3, 4.5)]);
    expect(component.scaleMax()).toBe(5);
    expect(component.barHeight(4.5)).toBe(90);
    expect(component.ticks().map(t => t.value)).toEqual([0, 2.5, 5]);
  });

  it('draws a bar only for days with spending, and labels just the tallest', () => {
    setDays([day(1, 2), day(2, 0), day(3, 4.5)]);
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelectorAll('.bar').length).toBe(2);
    expect(el.querySelectorAll('.peak').length).toBe(1);
    expect(el.querySelector('.peak')!.textContent!.trim()).toBe('£4.50');
  });

  it('lists the days with spending in the screen-reader table', () => {
    setDays([day(1, 2), day(2, 0), day(3, 4.5)]);
    const rows = fixture.nativeElement.querySelectorAll('tbody tr');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Thu 1 Oct');
  });

  it('shows the day and amount for the hovered bar, but nothing for days still to come', () => {
    setDays([day(1, 2), day(2, 0, true)]);
    component.hovered.set(0);
    expect(component.tooltip()?.label).toBe('Thu 1 Oct');
    expect(component.tooltip()?.left).toBe(25);
    expect(component.tooltip()?.align).toBe('start');

    component.hovered.set(1);
    expect(component.tooltip()).toBeNull();
  });

  it('shows an empty message for a month with no spending', () => {
    setDays([day(1, 0), day(2, 0)]);
    expect(fixture.nativeElement.querySelector('.empty').textContent).toContain('No expenses in October 2026');
  });

  it('labels the 1st and every 5th day', () => {
    expect([1, 2, 5, 9, 10, 31].map(d => component.showDayLabel(d))).toEqual([true, false, true, false, true, false]);
  });
});
