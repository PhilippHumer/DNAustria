import { Address } from './address.model';
import { Contact } from './contact.model';

export enum TargetAudience {
  Preschool = 10,
  PrimarySchool = 20,
  SecondaryLevel1 = 30,
  VocationalSchool = 40,
  SecondaryLevel2 = 50,
  Adults = 60,
  Families = 70,
  FemalesOnly = 80
}

export enum EventTopic {
  DigitizationIT = 100,
  ArtsCulture = 200,
  LanguagesLiterature = 300,
  MedicineHealth = 400,
  HistoryDemocracy = 500,
  EconomyLaw = 600,
  ScienceEnvironment = 700,
  MathData = 800
}

export enum EventStatus {
  Draft = 0,
  Approved = 1,
  Transferred = 2
}

export enum EventClassification {
  Scheduled = 0,
  OnDemand = 1
}

export interface Event {
  id: string;
  title: string;
  description: string;
  eventLink: string;
  targetAudience: TargetAudience[];
  topics: EventTopic[];
  dateStart: Date | string;
  dateEnd: Date | string;
  classification: EventClassification;
  fees: boolean;
  isOnline: boolean;
  organizationId: string;
  programName?: string;
  format?: string;
  schoolBookable?: boolean;
  ageMinimum?: number;
  ageMaximum?: number;
  location?: Address;
  contactId?: string;
  contact?: Contact;
  status: EventStatus;
  createdBy?: string;
  modifiedBy?: string;
  modifiedAt?: Date;
}
