import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

/** Marketing screenshot of the mobile app, drawn as a device frame. */
export type LandingPhoneScreen = 'dashboard' | 'expenses' | 'scan';

@Component({
  selector: 'app-landing-phone',
  standalone: true,
  templateUrl: './landing-phone.component.html',
  styleUrl: './landing-phone.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LandingPhoneComponent {
  /**
   * Screens are rendered here rather than projected: with view encapsulation
   * the .mini-* rules below would not reach content projected from a parent.
   */
  @Input({ required: true }) screen!: LandingPhoneScreen;
}
