import { ChangeDetectionStrategy, Component, OnDestroy, computed, signal } from '@angular/core';

@Component({
  selector: 'app-upload-receipt',
  standalone: true,
  templateUrl: './upload-receipt.component.html',
  styleUrl: './upload-receipt.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UploadReceiptComponent implements OnDestroy {
  selectedFile = signal<File | null>(null);
  previewUrl   = signal<string | null>(null);

  readonly formattedSize = computed(() => {
    const file = this.selectedFile();
    if (!file) return '';
    const kb = file.size / 1024;
    return kb < 1024 ? `${kb.toFixed(0)} KB` : `${(kb / 1024).toFixed(1)} MB`;
  });

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.setFile(file);
    input.value = '';
  }

  removeFile(): void {
    this.setFile(null);
  }

  private setFile(file: File | null): void {
    const current = this.previewUrl();
    if (current) URL.revokeObjectURL(current);

    this.selectedFile.set(file);
    this.previewUrl.set(file ? URL.createObjectURL(file) : null);
  }

  ngOnDestroy(): void {
    const current = this.previewUrl();
    if (current) URL.revokeObjectURL(current);
  }
}
