using System;
using System.Collections.Generic;

namespace Discover.DNAustria.Api
{
    public class EventDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public List<int>? Topics { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ContactId { get; set; }
        public List<int>? TargetAudience { get; set; }
        public bool IsOnline { get; set; }
        public string? EventLink { get; set; }
        public string Status { get; set; } = null!;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    public class ContactDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Guid? OrganizationId { get; set; }
    }

    public class OrganizationDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? AddressStreet { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressZip { get; set; }
        public int? RegionId { get; set; }
    }

    public class DNAustriaExportEventDto
    {
        public string EventTitle { get; set; } = null!;
        public string? EventDescription { get; set; }
        public DateTime? EventStart { get; set; }
        public DateTime? EventEnd { get; set; }
        public string? EventLink { get; set; }
        public List<int>? EventTopics { get; set; }
        public List<int>? EventTargetAudience { get; set; }
        public bool EventIsOnline { get; set; }
        public string OrganizationName { get; set; } = null!;
        public string? EventContactEmail { get; set; }
        public string? EventContactPhone { get; set; }
        public string? EventAddressStreet { get; set; }
        public string? EventAddressCity { get; set; }
        public string? EventAddressZip { get; set; }
        public string? EventAddressState { get; set; }
        public List<float>? Location { get; set; }
    }
}

