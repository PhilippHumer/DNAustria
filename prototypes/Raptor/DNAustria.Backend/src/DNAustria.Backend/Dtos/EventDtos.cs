using DNAustria.Backend.Models;

namespace DNAustria.Backend.Dtos;

public record EventListDto(Guid Id, string Title, DateTime DateStart, DateTime DateEnd, EventStatus Status);

public class EventDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EventLink { get; set; }
    public List<TargetAudience> TargetAudience { get; set; } = new();
    public List<EventTopic> Topics { get; set; } = new();
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public Classification Classification { get; set; }
    public bool Fees { get; set; }
    public bool IsOnline { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? ProgramName { get; set; }
    public string? Format { get; set; }
    public bool? SchoolBookable { get; set; }
    public int? AgeMinimum { get; set; }
    public int? AgeMaximum { get; set; }
    // Location object stored directly on the event (preferred). This replaces separate inline fields and LocationId in the DTO.
    public OrganizationCreateDto? Location { get; set; }
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }
    public EventStatus Status { get; set; }
}

public class OrganizationCreateDto
{
    public string Name { get; set; } = string.Empty; // OrganizationName (replaces LocationName)
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class ContactCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Org { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class EventCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EventLink { get; set; }
    public List<TargetAudience> TargetAudience { get; set; } = new();
    public List<EventTopic> Topics { get; set; } = new();
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public Classification Classification { get; set; }
    public bool Fees { get; set; }
    public bool IsOnline { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? ProgramName { get; set; }
    public string? Format { get; set; }
    public bool? SchoolBookable { get; set; }
    public int? AgeMinimum { get; set; }
    public int? AgeMaximum { get; set; }
    // Location object provided by client and saved on the Event.
    public OrganizationCreateDto? Location { get; set; }
    public Guid? ContactId { get; set; }
    public ContactCreateDto? Contact { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
}
