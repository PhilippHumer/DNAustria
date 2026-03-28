using DNAustria.Api.Dtos.Events;
using DNAustria.Domain;
using DalEvent = DNAustria.Dal.Models.Event;

namespace DNAustria.Api.MapperExtensions;

public static class EventsMapperExtensions
{
    public static IReadOnlyList<EventDto> ToDtos(this IEnumerable<Event> events)
    {
        return events.Select(e => new EventDto
            {
                Id = e.Id,
                Name = e.Name,
                AgeMaximum = e.AgeMaximum,
                AgeMinimum = e.AgeMinimum,
                Classification = (int)e.Classification,
                Contact = e.ContactId,
                Description = e.Description,
                EndDate = e.EndDate,
                HasFees = e.HasFees,
                IsOnline = e.IsOnline,
                Link = e.Link,
                Location = e.LocationId,
                Organization = e.OrganizationId,
                ProgramName = e.ProgramName,
                SchoolBookable = e.SchoolBookable,
                StartDate = e.StartDate,
                Status = (int)e.Status,
                TargetAudiences = e.TargetAudiences.Select(ta => ta.TargetAudience).ToList(),
                Topics = e.Topics.Select(t => t.Topic).ToList(),
                Format = e.Format
            })
            .ToList();
    }

    public static IReadOnlyList<PublicEventDto> ToPublicDtos(this IEnumerable<DalEvent> events)
    {
        return events.Select(e =>
        {
            var loc = e.LocationNavigation;
            var addr = loc?.AddressNavigation;
            var contact = e.ContactNavigation;
            var org = e.OrganizationNavigation;

            return new PublicEventDto
            {
                EventTitle = e.Name,
                EventDescription = e.Description,
                EventLink = e.Link,
                EventTargetAudience = e.EventTargetAudiences.Select(ta => ta.TargetAudience).ToList(),
                EventTopics = e.EventTopics.Select(t => t.Topic).ToList(),
                EventStart = DateTime.SpecifyKind(e.StartDate, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EventEnd = DateTime.SpecifyKind(e.EndDate, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EventClassification = ((EventClassification)e.Classification) switch
                {
                    EventClassification.Scheduled => "scheduled",
                    EventClassification.OnDemand => "on-demand",
                    _ => "unknown"
                },
                EventHasFees = e.HasFees,
                EventIsOnline = e.IsOnline,
                OrganizationName = org?.Name ?? "",
                ProgramName = e.ProgramName,
                EventFormat = e.Format,
                EventSchoolBookable = e.SchoolBookable,
                EventAgeMinimum = e.AgeMinimum,
                EventAgeMaximum = e.AgeMaximum,
                EventLocationName = loc?.Name,
                EventAddressStreet = addr?.Street,
                EventAddressCity = addr?.City,
                EventAddressZip = addr?.Zip,
                EventAddressState = addr?.State,
                EventContactName = contact?.Name,
                EventContactOrg = contact?.Organization,
                EventContactEmail = contact?.Email,
                EventContactPhone = contact?.Phone,
                Location = loc != null ? new List<double> { loc.Latitude, loc.Longitude } : null
            };
        }).ToList();
    }
}