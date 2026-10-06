import { Directive, ElementRef, HostListener, inject } from '@angular/core';

/**
 * Lets a mouse wheel scroll a sideways-scrolling row (chip rows, tabs) whose scrollbar is hidden,
 * like the mobile app's rows. Trackpads already scroll sideways, so only mostly-vertical wheel
 * movement is redirected, and only while the row can still move that way; at either end the
 * page scrolls as normal.
 */
@Directive({
  selector: '[appHorizontalWheelScroll]',
  standalone: true,
})
export class HorizontalWheelScrollDirective {
  private readonly el = inject(ElementRef<HTMLElement>);

  @HostListener('wheel', ['$event'])
  onWheel(event: WheelEvent): void {
    if (Math.abs(event.deltaY) <= Math.abs(event.deltaX)) return;

    const row: HTMLElement = this.el.nativeElement;
    const maxScroll = row.scrollWidth - row.clientWidth;
    const canMove = event.deltaY > 0 ? row.scrollLeft < maxScroll : row.scrollLeft > 0;
    if (maxScroll <= 0 || !canMove) return;

    row.scrollLeft += event.deltaY;
    event.preventDefault();
  }
}
