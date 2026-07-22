import { ChangeDetectionStrategy, Component, ElementRef, EventEmitter, OnDestroy, OnInit, Output, ViewChild, computed, inject, signal } from '@angular/core';
import { ImageResizeService } from '../../core/services/image-resize.service';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseTableService } from '../../core/services/expense-table.service';
import { convertIfHeic } from '../../core/utils/heic-converter';
import { todayLocalISODate } from '../../core/utils/date.utils';
import { ALL_CATEGORIES } from '../../core/utils/category.utils';
import { AddExpenseCommand, ExtractedExpense } from '../../core/models/expense.model';
import { SelectTablesPromptComponent } from '../expense-table/select-tables-prompt.component';

const MAX_DIMENSION = 1600;
const JPEG_QUALITY = 0.8;

type DraftExpense = ExtractedExpense & { id: number };

@Component({
  selector: 'app-upload-receipt',
  standalone: true,
  imports: [SelectTablesPromptComponent],
  templateUrl: './upload-receipt.component.html',
  styleUrl: './upload-receipt.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UploadReceiptComponent implements OnInit, OnDestroy {
  @Output() submitted = new EventEmitter<void>();
  @ViewChild('videoEl') videoRef?: ElementRef<HTMLVideoElement>;

  private readonly imageResizeService = inject(ImageResizeService);
  private readonly expenseService = inject(ExpenseService);
  private readonly expenseTableService = inject(ExpenseTableService);
  private nextDraftId = 0;

  readonly categories = ALL_CATEGORIES;
  readonly maxDate = todayLocalISODate();

  selectedFile = signal<File | null>(null);
  previewUrl   = signal<string | null>(null);
  processing   = signal(false);
  error        = signal<string | null>(null);

  cameraStream = signal<MediaStream | null>(null);
  cameraError  = signal<string | null>(null);

  extractedExpenses = signal<DraftExpense[]>([]);
  submitting        = signal(false);
  formError         = signal<string | null>(null);
  imageEnlarged     = signal(false);

  readonly step = signal<'review' | 'select-tables'>('review');
  private pendingCommands: AddExpenseCommand[] | null = null;

  readonly hasExtraction = computed(() => this.extractedExpenses().length > 0);

  readonly formattedSize = computed(() => {
    const file = this.selectedFile();
    if (!file) return '';
    const kb = file.size / 1024;
    return kb < 1024 ? `${kb.toFixed(0)} KB` : `${(kb / 1024).toFixed(1)} MB`;
  });

  ngOnInit(): void {
    void this.startCamera();
  }

  async onFileSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    this.stopCamera();
    await this.processFile(file);
  }

  capturePhoto(): void {
    const video = this.videoRef?.nativeElement;
    if (!video || !this.cameraStream()) return;

    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    this.stopCamera();

    canvas.toBlob(
      (blob) => {
        if (!blob) {
          this.error.set("Couldn't capture photo. Please try again.");
          return;
        }
        const file = new File([blob], `receipt-${Date.now()}.jpg`, { type: 'image/jpeg' });
        void this.processFile(file);
      },
      'image/jpeg',
      0.9
    );
  }

  private async startCamera(): Promise<void> {
    if (this.cameraStream()) return;

    this.cameraError.set(null);
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: 'environment' },
        audio: false,
      });
      this.cameraStream.set(stream);
    } catch {
      this.cameraError.set("Couldn't access the camera. You can still upload a photo instead.");
    }
  }

  private stopCamera(): void {
    this.cameraStream()?.getTracks().forEach(t => t.stop());
    this.cameraStream.set(null);
  }

  private async processFile(file: File): Promise<void> {
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

      const tableId = this.expenseTableService.tables()[0]?.id ?? 0;
      const items = await new Promise<ExtractedExpense[]>((resolve, reject) => {
        this.expenseService.extractReceipt(tableId, resizedFile, resolve, (msg) => reject(new Error(msg)));
      });
      this.extractedExpenses.set(items.map(e => ({ ...e, id: this.nextDraftId++ })));
    } catch {
      this.error.set("Couldn't process this image. Try a different photo or format.");
      void this.startCamera();
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
    void this.startCamera();
  }

  onAddExpenses(): void {
    const items = this.extractedExpenses();
    if (items.length === 0 || this.submitting()) return;

    this.formError.set(null);
    this.pendingCommands = items.map(i => ({
      expenseTableId: 0,
      amount: Math.round(i.amount * i.quantity * 100) / 100,
      category: i.category,
      description: i.description.trim(),
      date: i.date,
      merchant: i.merchant,
    }));
    this.step.set('select-tables');
  }

  onTablesConfirmed(tableIds: number[]): void {
    if (!this.pendingCommands) return;
    const items = this.extractedExpenses();

    this.submitting.set(true);
    this.expenseService.addExpensesBatchToTables(
      tableIds,
      this.pendingCommands,
      (results) => {
        this.submitting.set(false);

        const failedIndexes = new Set<number>();
        const failureMessages: string[] = [];

        results.forEach((result, tableIndex) => {
          for (const err of result.errors) {
            failedIndexes.add(err.index);
            const item = items[err.index];
            failureMessages.push(`${item?.description ?? 'Item'} (table ${tableIndex + 1}): ${err.errors.join(' ')}`);
          }
        });

        if (failedIndexes.size === 0) {
          this.pendingCommands = null;
          this.setFile(null);
          this.extractedExpenses.set([]);
          this.step.set('review');
          this.submitted.emit();
          return;
        }

        this.extractedExpenses.update(list => list.filter((_, i) => failedIndexes.has(i)));
        this.pendingCommands = null;
        this.step.set('review');
        this.formError.set(failureMessages.join(' '));
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }

  onTablesCancelled(): void {
    this.step.set('review');
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
    this.stopCamera();
  }
}
