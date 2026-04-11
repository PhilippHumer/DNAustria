import { EventDto } from './eventDto';

export interface PagedEventsDto {
  items: EventDto[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
