import { ChangeDetectionStrategy, Component } from '@angular/core';

/** Per-item price history chart plus repeat-purchase totals. */
@Component({
  selector: 'app-landing-insights',
  standalone: true,
  templateUrl: './landing-insights.component.html',
  styleUrls: ['../landing-shared.css', './landing-insights.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LandingInsightsComponent {
}
