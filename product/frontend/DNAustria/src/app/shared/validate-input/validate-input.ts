import { Component, input } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';

export interface ValidationMessage {
  key: string;
  message: string;
  /** Show this error as soon as the user starts typing (dirty), not only after blur (touched). */
  showOnDirty?: boolean;
}

@Component({
  selector: 'app-validate-input',
  template: `
    <ng-content />
    @for (vm of messages(); track vm.key) {
      @if (control()?.hasError(vm.key) && isVisible(vm)) {
        <div class="validation-error">{{ vm.message }}</div>
      }
    }
  `,
  styles: `
    :host {
      display: block;
    }

    .validation-error {
      color: #dc3545;
      font-size: 0.8rem;
      margin-top: 0.25rem;
    }
  `,
})
export class ValidateInput {
  readonly control = input.required<AbstractControl | null>();
  readonly messages = input<ValidationMessage[]>([]);

  protected isVisible(vm: ValidationMessage): boolean {
    const ctrl = this.control();
    if (!ctrl) return false;
    return vm.showOnDirty ? ctrl.dirty : ctrl.touched;
  }
}
