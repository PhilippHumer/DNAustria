
export interface Organization {
  id: string;
  name: string;
  city?: string;
  zip?: string;
  state?: string;
  street?: string;
  latitude?: number;
  longitude?: number;
  address?: string; // Formatted address string
}
