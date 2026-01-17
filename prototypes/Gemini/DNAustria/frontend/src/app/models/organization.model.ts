import { Address } from './address.model';

export interface Organization {
    id?: string;
    name: string;
    addressId?: string;
    address?: Address;
}
