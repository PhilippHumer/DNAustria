import { FormBuilder, FormGroup, AbstractControl, ValidationErrors, Validators } from '@angular/forms';

export interface ContactFormControls {
  name: string;
  email: string;
  phone: string;
  organization: string;
}

export class ContactFormGroupFactory {
  static create(fb: FormBuilder): FormGroup {
    const group = fb.nonNullable.group(
      {
        name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
        email: ['', [Validators.email]],
        phone: ['', [Validators.minLength(5), Validators.maxLength(20), Validators.pattern(/^\+?[0-9]*$/)]],
        organization: [''],
      },
      {
        validators: [ContactFormGroupFactory.emailOrPhoneRequired],
      },
    );
    return group;
  }

  static emailOrPhoneRequired(control: AbstractControl): ValidationErrors | null {
    const email = control.get('email')?.value?.trim() ?? '';
    const phone = control.get('phone')?.value?.trim() ?? '';

    if (!email && !phone) {
      return { emailOrPhoneRequired: true };
    }
    return null;
  }
}
