import { ChangeDetectionStrategy, Component, EventEmitter, OnDestroy, Output, computed, inject, signal } from '@angular/core';
import { ImageResizeService } from '../../core/services/image-resize.service';
import { ExpenseService } from '../../core/services/expense.service';
import { convertIfHeic } from '../../core/utils/heic-converter';
import { generateFakeExpenses } from '../../core/utils/fake-expense-generator';
import { todayLocalISODate } from '../../core/utils/date.utils';
import { ALL_CATEGORIES } from '../../core/utils/category.utils';
import { AddExpenseCommand, ExtractedExpense } from '../../core/models/expense.model';

const MAX_DIMENSION = 1600;
const JPEG_QUALITY = 0.8;

type DraftExpense = ExtractedExpense & { id: number };

@Component({
  selector: 'app-upload-receipt',
  standalone: true,
  templateUrl: './upload-receipt.component.html',
  styleUrl: './upload-receipt.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UploadReceiptComponent implements OnDestroy {
  @Output() submitted = new EventEmitter<void>();

  private readonly imageResizeService = inject(ImageResizeService);
  private readonly expenseService = inject(ExpenseService);
  private nextDraftId = 0;

  readonly categories = ALL_CATEGORIES;
  readonly maxDate = todayLocalISODate();

  selectedFile = signal<File | null>(null);
  previewUrl   = signal<string | null>(null);
  processing   = signal(false);
  error        = signal<string | null>(null);

  extractedExpenses = signal<DraftExpense[]>([]);
  submitting        = signal(false);
  formError         = signal<string | null>(null);
  imageEnlarged     = signal(false);

  readonly hasExtraction = computed(() => this.extractedExpenses().length > 0);

  readonly formattedSize = computed(() => {
    const file = this.selectedFile();
    if (!file) return '';
    const kb = file.size / 1024;
    return kb < 1024 ? `${kb.toFixed(0)} KB` : `${(kb / 1024).toFixed(1)} MB`;
  });

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    this.error.set(null);
    this.processing.set(true);

    try {
      const jpegFile = await convertIfHeic(file);
      const resizedBlob = await this.imageResizeService.resizeImage(jpegFile, {
        maxWidth: MAX_DIMENSION,
        maxHeight: MAX_DIMENSION,
        quality: JPEG_QUALITY,
      });
      const resizedFile = new File([resizedBlob], jpegFile.name, { type: 'image/jpeg' });

      this.setFile(resizedFile);
      this.extractedExpenses.set(
        generateFakeExpenses().map(e => ({ ...e, id: this.nextDraftId++ }))
      );
    } catch {
      this.error.set("Couldn't process this image. Try a different photo or format.");
    } finally {
      this.processing.set(false);
    }
  }

  updateItem(id: number, patch: Partial<ExtractedExpense>): void {
    this.extractedExpenses.update(items =>
      items.map(i => (i.id === id ? { ...i, ...patch } : i))
    );
  }

  removeItem(id: number): void {
    this.extractedExpenses.update(items => items.filter(i => i.id !== id));
  }

  removeFile(): void {
    this.error.set(null);
    this.formError.set(null);
    this.extractedExpenses.set([]);
    this.setFile(null);
  }

  onAddExpenses(): void {
    const items = this.extractedExpenses();
    if (items.length === 0 || this.submitting()) return;

    this.formError.set(null);
    this.submitting.set(true);

    const commands: AddExpenseCommand[] = items.map(i => ({
      amount: Math.round(i.amount * i.quantity * 100) / 100,
      category: i.category,
      description: i.description.trim(),
      date: i.date,
    }));

    this.expenseService.addExpensesBatch(
      commands,
      (result) => {
        this.submitting.set(false);

        if (result.errors.length === 0) {
          this.setFile(null);
          this.extractedExpenses.set([]);
          this.submitted.emit();
          return;
        }

        const failedIds = new Set(result.errors.map(e => items[e.index].id));
        this.extractedExpenses.update(list => list.filter(i => failedIds.has(i.id)));

        const messages = result.errors.map(e => {
          const item = items[e.index];
          return `${item.description}: ${e.errors.join(' ')}`;
        });
        this.formError.set(messages.join(' '));
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }

  openEnlargedImage(): void {
    this.imageEnlarged.set(true);
  }

  closeEnlargedImage(): void {
    this.imageEnlarged.set(false);
  }

  private setFile(file: File | null): void {
    const current = this.previewUrl();
    if (current) URL.revokeObjectURL(current);

    this.selectedFile.set(file);
    this.previewUrl.set(file ? URL.createObjectURL(file) : null);
    this.imageEnlarged.set(false);
  }

  ngOnDestroy(): void {
    const current = this.previewUrl();
    if (current) URL.revokeObjectURL(current);
  }
}
