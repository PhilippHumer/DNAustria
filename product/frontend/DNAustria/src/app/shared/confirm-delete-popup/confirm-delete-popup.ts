import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-delete-popup',
  template: `
    <div class="confirm-delete-backdrop">
      <div class="confirm-delete card shadow border-0">
        <div class="card-body">
          <h2 class="h5 fw-bold mb-2">{{ title() }}</h2>
          <p class="mb-0">{{ message() }}</p>
        </div>
        <div class="card-footer bg-white d-flex justify-content-end gap-2">
          @if (error()) {
            <div class="text-danger small me-auto align-self-center">{{ error() }}</div>
          }
          <button type="button" class="btn btn-outline-secondary" (click)="cancel.emit()" [disabled]="deleting()">
            Cancel
          </button>
          <button type="button" class="btn btn-danger" (click)="confirm.emit()" [disabled]="deleting()">
            @if (deleting()) {
              Deleting...
            } @else {
              Delete
            }
          </button>
        </div>
      </div>
    </div>
  `,
  styles: `
    .confirm-delete-backdrop {
      position: fixed;
      inset: 0;
      z-index: 1060;
      background: rgba(15, 23, 42, 0.45);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 1rem;
    }

    .confirm-delete {
      width: min(100%, 26rem);
      border-radius: 0.75rem;
    }
  `,
})
export class ConfirmDeletePopup {
  readonly title = input('Confirm Delete');
  readonly message = input('Do you really want to delete this entry?');
  readonly deleting = input(false);
  readonly error = input<string | null>(null);

  readonly confirm = output<void>();
  readonly cancel = output<void>();
}
