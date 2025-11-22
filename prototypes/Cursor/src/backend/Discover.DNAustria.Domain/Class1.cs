using System;
using System.Collections.Generic;

namespace Discover.DNAustria.Domain
{
    public enum EventStatus
    {
        Draft,
        Approved,
        Transferred
    }

    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        // Many-to-many relationships (assuming you have Topic and Audience entities)
        public List<int>? Topics { get; set; }
        public List<int>? TargetAudience { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        // Foreign keys
        public Guid? OrganizationId { get; set; }
        public Guid? ContactId { get; set; }

        // Navigational properties
        public Organization? Organization { get; set; }
        public Contact? Contact { get; set; }

        public bool IsOnline { get; set; }
        public string? EventLink { get; set; }
        public EventStatus Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    public class Contact
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Guid? OrganizationId { get; set; }
    }

    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? AddressStreet { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressZip { get; set; }
        public int? RegionId { get; set; }
    }
}
