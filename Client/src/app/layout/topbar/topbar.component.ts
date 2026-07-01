import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-topbar',
  standalone: true,
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.css',
})
export class TopbarComponent {
  @Input() pageTitle = '';
  @Output() menuClicked  = new EventEmitter<void>();
  @Output() addClicked   = new EventEmitter<void>();
}
