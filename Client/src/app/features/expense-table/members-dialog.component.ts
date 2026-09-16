import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { ExpenseTableMember } from '../../core/models/expense-table.model';
import { ExpenseTableService } from '../../core/services/expense-table.service';

@Component({
  selector: 'app-members-dialog',
  standalone: true,
  templateUrl: './members-dialog.component.html',
  styleUrl: './members-dialog.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MembersDialogComponent implements OnInit {
  @Input({ required: true }) tableId!: number;
  @Input() tableName = '';
  /** Admins get an "Invite someone" button that hands over to the share dialog. */
  @Input() canInvite = false;
  @Output() invite = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  private readonly expenseTableService = inject(ExpenseTableService);

  readonly members = signal<ExpenseTableMember[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.expenseTableService.getMembers(
      this.tableId,
      (members) => {
        this.members.set(members);
        this.loading.set(false);
      },
      (msg) => {
        this.error.set(msg);
        this.loading.set(false);
      },
    );
  }

  /** Avatar letter; the API only stores emails. */
  initial(member: ExpenseTableMember): string {
    return (member.email?.trim()[0] ?? '?').toUpperCase();
  }
}
