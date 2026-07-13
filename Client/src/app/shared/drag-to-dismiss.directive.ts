import { Directive, ElementRef, EventEmitter, HostListener, Output, inject } from '@angular/core';

const DISMISS_THRESHOLD_PX = 100;

@Directive({
  selector: '[appDragToDismiss]',
  standalone: true,
})
export class DragToDismissDirective {
  @Output() dismissed = new EventEmitter<void>();

  private readonly el = inject(ElementRef<HTMLElement>);

  private dragging = false;
  private startY = 0;
  private currentY = 0;

  @HostListener('pointerdown', ['$event'])
  onPointerDown(event: PointerEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.drawer-handle')) return;

    this.dragging = true;
    this.startY = event.clientY;
    this.currentY = 0;
    this.el.nativeElement.style.transition = 'none';
  }

  @HostListener('document:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (!this.dragging) return;

    const delta = event.clientY - this.startY;
    this.currentY = Math.max(0, delta);
    this.el.nativeElement.style.transform = `translateY(${this.currentY}px)`;
  }

  @HostListener('document:pointerup')
  onPointerUp(): void {
    if (!this.dragging) return;
    this.dragging = false;

    this.el.nativeElement.style.transition = '';

    if (this.currentY > DISMISS_THRESHOLD_PX) {
      this.dismissed.emit();
    } else {
      this.el.nativeElement.style.transform = '';
    }
  }
}
