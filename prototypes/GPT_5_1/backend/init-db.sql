-- Initialize database schema for DiscoverDNAustria
-- Derived from Domain/Entities (Organization, Contact, Event, EventStatus)

-- Use default public schema
SET client_min_messages = warning;

-- Optional: ensure uuid type is available (Postgres has uuid built-in)
-- If you need server-generated UUIDs, uncomment the extension
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create tables if they do not already exist

DO $$
BEGIN
    -- Falls vorher unquoted lowercase Tabellen existieren, auf PascalCase umbenennen
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='organizations')
       AND NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Organizations') THEN
        EXECUTE 'ALTER TABLE public.organizations RENAME TO "Organizations"';
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='contacts')
       AND NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Contacts') THEN
        EXECUTE 'ALTER TABLE public.contacts RENAME TO "Contacts"';
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='events')
       AND NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Events') THEN
        EXECUTE 'ALTER TABLE public.events RENAME TO "Events"';
    END IF;
END $$;

DO $$
BEGIN
    -- Spalten in Organizations umbenennen
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Organizations') THEN
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='Id') THEN
            EXECUTE 'ALTER TABLE public."Organizations" RENAME COLUMN id TO "Id"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='name') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='Name') THEN
            EXECUTE 'ALTER TABLE public."Organizations" RENAME COLUMN name TO "Name"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='address_street') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='AddressStreet') THEN
            EXECUTE 'ALTER TABLE public."Organizations" RENAME COLUMN address_street TO "AddressStreet"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='address_city') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='AddressCity') THEN
            EXECUTE 'ALTER TABLE public."Organizations" RENAME COLUMN address_city TO "AddressCity"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='address_zip') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='AddressZip') THEN
            EXECUTE 'ALTER TABLE public."Organizations" RENAME COLUMN address_zip TO "AddressZip"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='region_id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Organizations' AND column_name='RegionId') THEN
            EXECUTE 'ALTER TABLE public."Organizations" RENAME COLUMN region_id TO "RegionId"';
        END IF;
    END IF;

    -- Spalten in Contacts umbenennen
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Contacts') THEN
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='Id') THEN
            EXECUTE 'ALTER TABLE public."Contacts" RENAME COLUMN id TO "Id"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='name') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='Name') THEN
            EXECUTE 'ALTER TABLE public."Contacts" RENAME COLUMN name TO "Name"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='email') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='Email') THEN
            EXECUTE 'ALTER TABLE public."Contacts" RENAME COLUMN email TO "Email"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='phone') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='Phone') THEN
            EXECUTE 'ALTER TABLE public."Contacts" RENAME COLUMN phone TO "Phone"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='organization_id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Contacts' AND column_name='OrganizationId') THEN
            EXECUTE 'ALTER TABLE public."Contacts" RENAME COLUMN organization_id TO "OrganizationId"';
        END IF;
    END IF;

    -- Spalten in Events umbenennen
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Events') THEN
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='Id') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN id TO "Id"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='title') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='Title') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN title TO "Title"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='description') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='Description') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN description TO "Description"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='topics') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='Topics') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN topics TO "Topics"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='date_start') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='DateStart') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN date_start TO "DateStart"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='date_end') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='DateEnd') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN date_end TO "DateEnd"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='organization_id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='OrganizationId') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN organization_id TO "OrganizationId"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='contact_id') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='ContactId') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN contact_id TO "ContactId"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='target_audience') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='TargetAudience') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN target_audience TO "TargetAudience"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='is_online') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='IsOnline') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN is_online TO "IsOnline"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='event_link') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='EventLink') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN event_link TO "EventLink"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='status') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='Status') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN status TO "Status"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='created_by') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='CreatedBy') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN created_by TO "CreatedBy"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='modified_by') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='ModifiedBy') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN modified_by TO "ModifiedBy"';
        END IF;
        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='modified_at') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='Events' AND column_name='ModifiedAt') THEN
            EXECUTE 'ALTER TABLE public."Events" RENAME COLUMN modified_at TO "ModifiedAt"';
        END IF;
    END IF;
END $$;

-- Organizations (PascalCase)
CREATE TABLE IF NOT EXISTS public."Organizations" (
    "Id" UUID PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "AddressStreet" TEXT NULL,
    "AddressCity" TEXT NULL,
    "AddressZip" TEXT NULL,
    "RegionId" INTEGER NULL
);

-- Contacts
CREATE TABLE IF NOT EXISTS public."Contacts" (
    "Id" UUID PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Email" TEXT NULL,
    "Phone" TEXT NULL,
    "OrganizationId" UUID NULL,
    CONSTRAINT fk_contacts_organization
        FOREIGN KEY ("OrganizationId")
        REFERENCES public."Organizations" ("Id")
        ON DELETE SET NULL
);

-- Events
CREATE TABLE IF NOT EXISTS public."Events" (
    "Id" UUID PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Topics" INTEGER[] NULL,
    "DateStart" TIMESTAMPTZ NULL,
    "DateEnd" TIMESTAMPTZ NULL,
    "OrganizationId" UUID NULL,
    "ContactId" UUID NULL,
    "TargetAudience" INTEGER[] NULL,
    "IsOnline" BOOLEAN NOT NULL DEFAULT FALSE,
    "EventLink" TEXT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "CreatedBy" TEXT NULL,
    "ModifiedBy" TEXT NULL,
    "ModifiedAt" TIMESTAMPTZ NOT NULL DEFAULT (NOW()),
    CONSTRAINT fk_events_organization
        FOREIGN KEY ("OrganizationId") REFERENCES public."Organizations" ("Id") ON DELETE SET NULL,
    CONSTRAINT fk_events_contact
        FOREIGN KEY ("ContactId") REFERENCES public."Contacts" ("Id") ON DELETE SET NULL,
    CONSTRAINT chk_status_valid CHECK ("Status" IN (0,1,2))
);

-- Helpful indexes
CREATE INDEX IF NOT EXISTS idx_contacts_org ON public."Contacts" ("OrganizationId");
CREATE INDEX IF NOT EXISTS idx_events_org ON public."Events" ("OrganizationId");
CREATE INDEX IF NOT EXISTS idx_events_contact ON public."Events" ("ContactId");
CREATE INDEX IF NOT EXISTS idx_events_modified_at ON public."Events" ("ModifiedAt" DESC);
CREATE INDEX IF NOT EXISTS idx_events_status ON public."Events" ("Status");

-- Health-check view/table (optional). If your API calls /public/health, it likely just checks the app.
-- Uncomment if you want a simple table to verify DB connectivity.
-- CREATE TABLE IF NOT EXISTS public.health_check (
--     id SERIAL PRIMARY KEY,
--     checked_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
-- );

-- Seed data (idempotent)
-- Predefined UUIDs to allow stable FK references
-- Organizations
INSERT INTO public."Organizations" ("Id", "Name", "AddressStreet", "AddressCity", "AddressZip", "RegionId") VALUES
 ('11111111-1111-1111-1111-111111111111','Tech Hub Linz','Hauptstraße 1','Linz','4020',7),
 ('22222222-2222-2222-2222-222222222222','Innovation Center Vienna','Ringstraße 10','Wien','1010',9),
 ('33333333-3333-3333-3333-333333333333','Startup Salzburg','Universitätsplatz 5','Salzburg','5020',5)
ON CONFLICT ("Id") DO NOTHING;

-- Contacts (linked to organizations)
INSERT INTO public."Contacts" ("Id","Name","Email","Phone","OrganizationId") VALUES
 ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa','Jane Doe','jane.doe@techhub.at','+43 660 1234567','11111111-1111-1111-1111-111111111111'),
 ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb','Max Mustermann','max@innovation.vienna','+43 1 9876543','22222222-2222-2222-2222-222222222222'),
 ('cccccccc-cccc-cccc-cccc-cccccccccccc','Anna Bauer','anna.bauer@startup-sbg.at',NULL,'33333333-3333-3333-3333-333333333333')
ON CONFLICT ("Id") DO NOTHING;

-- Events (linked to organizations and contacts)
INSERT INTO public."Events" ("Id","Title","Description","Topics","DateStart","DateEnd","OrganizationId","ContactId","TargetAudience","IsOnline","EventLink","Status","CreatedBy","ModifiedBy","ModifiedAt") VALUES
 ('eeeeeeee-0000-0000-0000-000000000001','AI Meetup Linz','Monatliches Treffen zur Diskussion von KI-Trends.',ARRAY[1,2,5],NOW()+INTERVAL '7 days',NOW()+INTERVAL '7 days'+INTERVAL '2 hours','11111111-1111-1111-1111-111111111111','aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',ARRAY[10,20],TRUE,'https://events.techhub.at/ai-meetup',1,'seed-script','seed-script',NOW()),
 ('eeeeeeee-0000-0000-0000-000000000002','Innovation Day Vienna','Konferenz zu Innovation und Digitalisierung.',ARRAY[3,8],NOW()+INTERVAL '14 days',NOW()+INTERVAL '14 days'+INTERVAL '8 hours','22222222-2222-2222-2222-222222222222','bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',ARRAY[30],FALSE,NULL,0,'seed-script','seed-script',NOW()),
 ('eeeeeeee-0000-0000-0000-000000000003','Startup Pitch Night Salzburg','Abendveranstaltung mit Pitches lokaler Startups.',ARRAY[4],NOW()+INTERVAL '21 days',NOW()+INTERVAL '21 days'+INTERVAL '3 hours','33333333-3333-3333-3333-333333333333','cccccccc-cccc-cccc-cccc-cccccccccccc',ARRAY[15,25,35],TRUE,'https://startup-sbg.at/pitch-night',2,'seed-script','seed-script',NOW())
ON CONFLICT ("Id") DO NOTHING;
