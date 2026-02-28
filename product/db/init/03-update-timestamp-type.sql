BEGIN;

ALTER TABLE event
  ALTER COLUMN start_date TYPE timestamptz
  USING start_date AT TIME ZONE 'UTC';

ALTER TABLE event
  ALTER COLUMN end_date TYPE timestamptz
  USING end_date AT TIME ZONE 'UTC';

COMMIT;