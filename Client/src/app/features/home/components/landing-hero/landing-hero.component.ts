import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { LandingPhoneComponent } from '../landing-phone/landing-phone.component';

/** Landing hero: headline, blurb, calls to action and two app screenshots. */
@Component({
  selector: 'app-landing-hero',
  standalone: true,
  imports: [LandingPhoneComponent],
  templateUrl: './landing-hero.component.html',
  styleUrls: ['../landing-shared.css', './landing-hero.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LandingHeroComponent {
  /** Bubbles up so HomeComponent owns the single call to the Auth0 flow. */
  @Output() getStarted = new EventEmitter<void>();
}
