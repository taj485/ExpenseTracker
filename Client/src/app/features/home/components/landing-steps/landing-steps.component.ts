import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LandingPhoneComponent } from '../landing-phone/landing-phone.component';

/** "Three steps, about ten seconds" - scan, check, save. */
@Component({
  selector: 'app-landing-steps',
  standalone: true,
  imports: [LandingPhoneComponent],
  templateUrl: './landing-steps.component.html',
  styleUrls: ['../landing-shared.css', './landing-steps.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LandingStepsComponent {
}
