import { ChangeDetectionStrategy, Component, computed, input, linkedSignal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { merchantInitials, merchantInitialsColor, merchantLogoPaths } from '../../core/utils/merchant.utils';

/** Logo box size in CSS pixels — the image is requested at 2x for sharpness. */
const LOGO_SIZE_PX = 32;

@Component({
  selector: 'app-merchant-logo',
  standalone: true,
  templateUrl: './merchant-logo.component.html',
  styleUrl: './merchant-logo.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MerchantLogoComponent {
  readonly merchant = input<string | null>(null);
  /** Domain from the merchant reference table; skips the domain guessing when present. */
  readonly website = input<string | null>(null);

  /** Which candidate path we're on. Resets whenever the merchant changes. */
  private readonly attempt = linkedSignal<string, number>({
    source: () => `${this.merchant()}|${this.website()}`,
    computation: () => 0,
  });

  private readonly paths = computed(() => merchantLogoPaths(this.merchant(), this.website()));

  /** Null once every candidate has failed, which is what shows the merchant name instead. */
  readonly logoUrl = computed(() => {
    const path = this.paths()[this.attempt()];
    if (!path) return null;

    // fallback=404 is essential: without it logo.dev answers 202 with a generated monogram,
    // the image loads successfully, and the name fallback below is never reached.
    return `https://img.logo.dev/${path}?token=${environment.logoDev.token}`
      + `&size=${LOGO_SIZE_PX * 2}&format=png&fallback=404`;
  });

  /** Monogram fallback, used once every candidate logo URL has failed. */
  readonly initials = computed(() => merchantInitials(this.merchant()));
  readonly initialsColor = computed(() => merchantInitialsColor(this.merchant()));

  onLogoError(): void {
    this.attempt.update(n => n + 1);
  }
}
