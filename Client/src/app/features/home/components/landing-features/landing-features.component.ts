import { ChangeDetectionStrategy, Component } from '@angular/core';

/** "Know where the money actually goes" - the four feature cards. */
@Component({
  selector: 'app-landing-features',
  standalone: true,
  templateUrl: './landing-features.component.html',
  styleUrls: ['../landing-shared.css', './landing-features.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LandingFeaturesComponent {
}
