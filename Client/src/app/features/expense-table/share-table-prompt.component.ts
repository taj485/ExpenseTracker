import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ExpenseTableService } from '../../core/services/expense-table.service';

@Component({
  selector: 'app-share-table-prompt',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './share-table-prompt.component.html',
  styleUrl: './share-table-prompt.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ShareTablePromptComponent {
  @Input({ required: true }) tableId!: number;
  @Output() invited = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  private readonly expenseTableService = inject(ExpenseTableService);

  email = signal('');
  makeAdmin = signal(false);
  submitting = signal(false);
  formError = signal<string | null>(null);

  onSubmit(): void {
    const email = this.email().trim();

    if (!email) {
      this.formError.set('Email is required.');
      return;
    }

    this.formError.set(null);
    this.submitting.set(true);

    this.expenseTableService.inviteUser(
      { expenseTableId: this.tableId, inviteeEmail: email, isAdmin: this.makeAdmin() },
      () => {
        this.submitting.set(false);
        this.email.set('');
        this.makeAdmin.set(false);
        this.invited.emit();
      },
      (msg) => {
        this.submitting.set(false);
        this.formError.set(msg);
      }
    );
  }
}
