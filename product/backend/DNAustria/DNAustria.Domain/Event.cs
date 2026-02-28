namespace DNAustria.Domain;

public class Event
{
    protected Event() { }

    public Event(
        string name,
        string description,
        string link,
        DateTime startDate,
        DateTime endDate,
        EventClassification classification,
        EventStatus status,
        bool hasFees,
        bool isOnline,
        string programName,
        string format,
        bool schoolBookable,
        int ageMinimum,
        int ageMaximum,
        int? organizationId = null,
        int? locationId = null,
        int? contactId = null,
        IEnumerable<int>? targetAudienceIds = null,
        IEnumerable<int>? topicIds = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty");

        if (string.IsNullOrWhiteSpace(link))
            throw new ArgumentException("Link cannot be empty");

        if (string.IsNullOrWhiteSpace(programName))
            throw new ArgumentException("ProgramName cannot be empty");

        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("Format cannot be empty");

        if (endDate < startDate)
            throw new ArgumentException("EndDate cannot be empty");

        if (ageMinimum < 0 || ageMaximum < 0 || ageMinimum > 999 || ageMaximum > 999)
            throw new ArgumentOutOfRangeException(nameof(ageMinimum), "Age has to be between 0 and 999");

        if (ageMinimum > ageMaximum)
            throw new ArgumentException("AgeMinimum cannot be greater than AgeMaximum");

        Name = name.Trim();
        Description = description.Trim();
        Link = link.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Classification = classification;
        Status = status;
        HasFees = hasFees;
        IsOnline = isOnline;
        ProgramName = programName.Trim();
        Format = format.Trim();
        SchoolBookable = schoolBookable;
        AgeMinimum = ageMinimum;
        AgeMaximum = ageMaximum;
        OrganizationId = organizationId;
        LocationId = locationId;
        ContactId = contactId;

        TargetAudiences = (targetAudienceIds ?? Enumerable.Empty<int>())
            .Distinct()
            .Select(id => new EventTargetAudience { TargetAudience = id })
            .ToList();

        Topics = (topicIds ?? Enumerable.Empty<int>())
            .Distinct()
            .Select(id => new EventTopic { Topic = id })
            .ToList();
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public EventClassification Classification { get; private set; }
    public EventStatus Status { get; private set; }

    public bool HasFees { get; private set; }
    public bool IsOnline { get; private set; }

    public int? OrganizationId { get; private set; }
    public int? LocationId { get; private set; }
    public int? ContactId { get; private set; }

    public string ProgramName { get; private set; }
    public string Format { get; private set; }
    public bool SchoolBookable { get; private set; }

    public int AgeMinimum { get; private set; }
    public int AgeMaximum { get; private set; }

    public List<EventTargetAudience> TargetAudiences { get; private set; }
    public List<EventTopic> Topics { get; private set; }
}