BEGIN;

ALTER TABLE address
    ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE organization
    ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE contact
    ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE location
    ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE event
    ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE contact DROP CONSTRAINT IF EXISTS contact_email_key;
ALTER TABLE contact DROP CONSTRAINT IF EXISTS contact_phone_key;

-- Create partial unique indexes (only for active records)
CREATE UNIQUE INDEX IF NOT EXISTS ux_contact_email_active
    ON contact(email)
    WHERE is_deleted = FALSE;

CREATE UNIQUE INDEX IF NOT EXISTS ux_contact_phone_active
    ON contact(phone)
    WHERE is_deleted = FALSE;
	
COMMIT;