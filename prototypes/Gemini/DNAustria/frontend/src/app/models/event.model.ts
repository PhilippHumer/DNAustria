import { Address } from './address.model';
import { Contact } from './contact.model';
import { Organization } from './organization.model';
import { EventClassification, EventStatus, EventTopic, TargetAudience } from './enums';

export interface Event {
    id?: string;
    title: string;
    description: string;
    eventLink?: string;
    targetAudience: TargetAudience[];
    topics: EventTopic[];
    dateStart: string; // ISO string
    dateEnd: string;
    classification: EventClassification;
    fees: boolean;
    isOnline: boolean;
    organizationId?: string;
    organization?: Organization;
    programName?: string;
    format?: string;
    schoolBookable?: boolean;
    ageMinimum?: number;
    ageMaximum?: number;
    locationId?: string;
    location?: Address;
    contactId?: string;
    contact?: Contact;
    status: EventStatus;
    createdBy?: string;
    modifiedBy?: string;
    modifiedAt?: string;
}
