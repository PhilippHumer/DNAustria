export const EVENT_STATUS_LABELS: Record<number, string> = {
  0: 'Draft',
  1: 'Published',
  2: 'Approved',
};

export const EVENT_CLASSIFICATION_LABELS: Record<number, string> = {
  0: 'Scheduled',
  1: 'On-Demand',
};

export const EVENT_STATUS_OPTIONS = Object.entries(EVENT_STATUS_LABELS).map(([value, label]) => ({
  value: Number(value),
  label,
}));

export const EVENT_CLASSIFICATION_OPTIONS = Object.entries(EVENT_CLASSIFICATION_LABELS).map(
  ([value, label]) => ({
    value: Number(value),
    label,
  }),
);

export const EVENT_TARGET_AUDIENCE_OPTIONS = [
  { value: 10, label: 'Preschool children', description: 'Elementary level' },
  { value: 20, label: 'School children', description: 'Primary level' },
  { value: 30, label: 'Teenagers', description: 'Secondary level I' },
  { value: 40, label: 'Teenagers', description: 'Vocational schools, PTS' },
  { value: 50, label: 'Teenagers', description: 'Secondary level II' },
  { value: 60, label: 'Adults', description: 'Adult learners and participants' },
  { value: 70, label: 'Families', description: 'Suitable for family participation' },
  { value: 80, label: 'Girls/Women only', description: 'Intended only for girls or women' },
];

export const EVENT_TOPIC_OPTIONS = [
  { value: 100, label: 'Digitalization, AI, IT, Technology' },
  { value: 200, label: 'Art, Culture' },
  { value: 300, label: 'Languages, Literature' },
  { value: 400, label: 'Medicine, Health' },
  { value: 500, label: 'History, Democracy, Society' },
  { value: 600, label: 'Business, Law' },
  { value: 700, label: 'Natural Science, Climate, Environment' },
  { value: 800, label: 'Mathematics, Numbers, Data' },
];

export function getEventTargetAudienceOption(value: number) {
  return EVENT_TARGET_AUDIENCE_OPTIONS.find((option) => option.value === value);
}

export function getEventTopicOption(value: number) {
  return EVENT_TOPIC_OPTIONS.find((option) => option.value === value);
}

export function getEventStatusLabel(status: number): string {
  return EVENT_STATUS_LABELS[status] ?? `Status ${status}`;
}

export function getEventClassificationLabel(classification: number): string {
  return EVENT_CLASSIFICATION_LABELS[classification] ?? `Classification ${classification}`;
}

export function getEventStatusBadgeClass(status: number): string {
  switch (status) {
    case 1:
      return 'bg-success-subtle text-success-emphasis border-success-subtle';
    case 2:
      return 'bg-secondary-subtle text-secondary-emphasis border-secondary-subtle';
    case 3:
      return 'bg-danger-subtle text-danger-emphasis border-danger-subtle';
    default:
      return 'bg-warning-subtle text-warning-emphasis border-warning-subtle';
  }
}

export function toDatetimeLocalValue(value: string | null | undefined): string {
  if (!value) {
    return '';
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return '';
  }

  const pad = (part: number) => part.toString().padStart(2, '0');

  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

export function fromDatetimeLocalValue(value: string): string {
  if (!value) {
    return '';
  }

  return new Date(value).toISOString();
}

export function parseNumberList(value: string): number[] {
  return value
    .split(',')
    .map((part) => Number(part.trim()))
    .filter((part) => Number.isInteger(part));
}

export function formatNumberList(values: number[] | null | undefined): string {
  return values?.join(', ') ?? '';
}
