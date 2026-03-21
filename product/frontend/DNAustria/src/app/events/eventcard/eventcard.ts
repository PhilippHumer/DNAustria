import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventDto } from '../../api/model/eventDto';
import { getEventStatusBadgeClass, getEventStatusLabel } from '../event-utils';

@Component({
  selector: 'app-eventcard',
  imports: [DatePipe, RouterLink],
  templateUrl: './eventcard.html',
  styleUrl: './eventcard.css',
})
export class Eventcard {
  private static readonly DESCRIPTION_PREVIEW_LIMIT = 999;

  readonly event = input.required<EventDto>();
  readonly editClicked = output<void>();
  readonly deleteClicked = output<void>();

  protected statusLabel(status: number): string {
    return getEventStatusLabel(status);
  }

  protected statusBadgeClass(status: number): string {
    return getEventStatusBadgeClass(status);
  }

  protected descriptionPreview(description: string | null | undefined): string {
    const normalizedDescription = description?.trim();

    if (!normalizedDescription) {
      return 'No description available.';
    }

    if (normalizedDescription.length <= Eventcard.DESCRIPTION_PREVIEW_LIMIT) {
      return normalizedDescription;
    }

    return `${normalizedDescription.slice(0, Eventcard.DESCRIPTION_PREVIEW_LIMIT).trimEnd()}...`;
  }

  protected onEditClick(): void {
    this.editClicked.emit();
  }

  protected onDeleteClick(): void {
    this.deleteClicked.emit();
  }
}
