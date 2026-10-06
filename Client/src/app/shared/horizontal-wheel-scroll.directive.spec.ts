import { ElementRef } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { HorizontalWheelScrollDirective } from './horizontal-wheel-scroll.directive';

describe('HorizontalWheelScrollDirective', () => {
  let row: HTMLElement;
  let directive: HorizontalWheelScrollDirective;

  /** A row 100px wide holding 300px of content, scrolled to `scrollLeft`. */
  function setUp(scrollLeft: number, scrollWidth = 300): void {
    row = document.createElement('div');
    Object.defineProperty(row, 'clientWidth', { value: 100 });
    Object.defineProperty(row, 'scrollWidth', { value: scrollWidth });
    row.scrollLeft = scrollLeft;

    TestBed.configureTestingModule({ providers: [{ provide: ElementRef, useValue: new ElementRef(row) }] });
    directive = TestBed.runInInjectionContext(() => new HorizontalWheelScrollDirective());
  }

  function wheel(deltaY: number, deltaX = 0): WheelEvent {
    const event = new WheelEvent('wheel', { deltaY, deltaX, cancelable: true });
    directive.onWheel(event);
    return event;
  }

  it('turns vertical wheel movement into sideways scrolling', () => {
    setUp(0);
    const event = wheel(40);
    expect(row.scrollLeft).toBe(40);
    expect(event.defaultPrevented).toBe(true);
  });

  it('leaves trackpad sideways scrolling alone', () => {
    setUp(0);
    const event = wheel(5, 30);
    expect(row.scrollLeft).toBe(0);
    expect(event.defaultPrevented).toBe(false);
  });

  it('lets the page scroll once the row reaches its end', () => {
    setUp(200);
    expect(wheel(40).defaultPrevented).toBe(false);
  });

  it('lets the page scroll when the row is at its start and the wheel moves up', () => {
    setUp(0);
    expect(wheel(-40).defaultPrevented).toBe(false);
  });

  it('does nothing when everything fits', () => {
    setUp(0, 100);
    expect(wheel(40).defaultPrevented).toBe(false);
  });
});
