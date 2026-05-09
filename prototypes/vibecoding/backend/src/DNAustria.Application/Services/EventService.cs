using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;
using DNAustria.Domain.Enums;

namespace DNAustria.Application.Services;

public class EventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EventDto>> GetAllAsync(string? titleFilter, CancellationToken ct = default)
    {
        var events = await _repository.GetAllAsync(titleFilter?.Trim(), ct);
        return events.Select(ToDto).ToList();
    }

    public async Task<EventDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var ev = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Event {id} not found.");
        return ToDto(ev);
    }

    public async Task<List<EventDto>> GetPublicAsync(CancellationToken ct = default)
    {
        var events = await _repository.GetPublicAsync(ct);
        return events.Select(ToDto).ToList();
    }

    public async Task<EventDto> CreateAsync(CreateEventRequest request, CancellationToken ct = default)
    {
        var (title, description) = ValidateAndTrimRequired(request.Title, request.Description);

        if (request.DateStart == null)
            throw new ArgumentException("DateStart is required.");
        if (request.DateEnd == null)
            throw new ArgumentException("DateEnd is required.");
        if (request.DateEnd < request.DateStart)
            throw new ArgumentException("DateEnd must be greater than or equal to DateStart.");

        if (request.Fees == null)
            throw new ArgumentException("Fees is required.");
        if (request.IsOnline == null)
            throw new ArgumentException("IsOnline is required.");
        if (request.OrganizationId == null || request.OrganizationId == Guid.Empty)
            throw new ArgumentException("OrganizationId is required.");

        var classification = ParseClassification(request.Classification);

        var targetAudience = ValidateTargetAudience(request.TargetAudience);
        var topics = ValidateTopics(request.Topics);

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            EventLink = request.EventLink?.Trim(),
            TargetAudience = targetAudience,
            Topics = topics,
            DateStart = request.DateStart.Value,
            DateEnd = request.DateEnd.Value,
            Classification = classification,
            Fees = request.Fees.Value,
            IsOnline = request.IsOnline.Value,
            OrganizationId = request.OrganizationId.Value,
            ProgramName = request.ProgramName?.Trim(),
            Format = request.Format?.Trim(),
            SchoolBookable = request.SchoolBookable,
            AgeMinimum = request.AgeMinimum,
            AgeMaximum = request.AgeMaximum,
            LocationId = request.LocationId,
            ContactId = request.ContactId,
            Status = EventStatus.Draft,
            CreatedBy = request.CreatedBy?.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddAsync(ev, ct);
        await _repository.SaveChangesAsync(ct);

        return ToDto(ev);
    }

    public async Task<EventDto> UpdateAsync(Guid id, UpdateEventRequest request, CancellationToken ct = default)
    {
        var ev = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Event {id} not found.");

        var (title, description) = ValidateAndTrimRequired(request.Title, request.Description);

        if (request.DateStart == null)
            throw new ArgumentException("DateStart is required.");
        if (request.DateEnd == null)
            throw new ArgumentException("DateEnd is required.");
        if (request.DateEnd < request.DateStart)
            throw new ArgumentException("DateEnd must be greater than or equal to DateStart.");

        if (request.Fees == null)
            throw new ArgumentException("Fees is required.");
        if (request.IsOnline == null)
            throw new ArgumentException("IsOnline is required.");
        if (request.OrganizationId == null || request.OrganizationId == Guid.Empty)
            throw new ArgumentException("OrganizationId is required.");

        var classification = ParseClassification(request.Classification);
        var targetAudience = ValidateTargetAudience(request.TargetAudience);
        var topics = ValidateTopics(request.Topics);

        ev.Title = title;
        ev.Description = description;
        ev.EventLink = request.EventLink?.Trim();
        ev.TargetAudience = targetAudience;
        ev.Topics = topics;
        ev.DateStart = request.DateStart.Value;
        ev.DateEnd = request.DateEnd.Value;
        ev.Classification = classification;
        ev.Fees = request.Fees.Value;
        ev.IsOnline = request.IsOnline.Value;
        ev.OrganizationId = request.OrganizationId.Value;
        ev.ProgramName = request.ProgramName?.Trim();
        ev.Format = request.Format?.Trim();
        ev.SchoolBookable = request.SchoolBookable;
        ev.AgeMinimum = request.AgeMinimum;
        ev.AgeMaximum = request.AgeMaximum;
        ev.LocationId = request.LocationId;
        ev.ContactId = request.ContactId;
        ev.ModifiedBy = request.ModifiedBy?.Trim();
        ev.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);

        return ToDto(ev);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var ev = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Event {id} not found.");

        ev.IsDeleted = true;
        ev.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);
    }

    public async Task<EventDto> UpdateStatusAsync(Guid id, UpdateEventStatusRequest request, CancellationToken ct = default)
    {
        var ev = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Event {id} not found.");

        if (!Enum.TryParse<EventStatus>(request.Status, ignoreCase: true, out var newStatus))
            throw new ArgumentException($"Invalid status value: '{request.Status}'.");

        ValidateStatusTransition(ev.Status, newStatus);

        ev.Status = newStatus;
        ev.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);

        return ToDto(ev);
    }

    private static void ValidateStatusTransition(EventStatus current, EventStatus next)
    {
        var allowed = current switch
        {
            EventStatus.Draft => new[] { EventStatus.Approved },
            EventStatus.Approved => new[] { EventStatus.Draft, EventStatus.Transferred },
            EventStatus.Transferred => Array.Empty<EventStatus>(),
            _ => Array.Empty<EventStatus>()
        };

        if (!allowed.Contains(next))
            throw new ArgumentException($"Status transition from '{current}' to '{next}' is not allowed.");
    }

    private static (string title, string description) ValidateAndTrimRequired(string? rawTitle, string? rawDescription)
    {
        var title = rawTitle?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(title) || title.Length > 255)
            throw new ArgumentException("Title is required and must be at most 255 characters.");

        var description = rawDescription?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Description is required.");

        return (title, description);
    }

    private static Classification ParseClassification(string? raw)
    {
        if (!Enum.TryParse<Classification>(raw, ignoreCase: true, out var classification))
            throw new ArgumentException($"Invalid classification value: '{raw}'. Valid values: Scheduled, OnDemand.");
        return classification;
    }

    private static List<int> ValidateTargetAudience(List<int>? list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("TargetAudience is required and must contain at least one value.");
        var valid = Enum.GetValues<TargetAudience>().Select(e => (int)e).ToHashSet();
        foreach (var v in list)
        {
            if (!valid.Contains(v))
                throw new ArgumentException($"Invalid TargetAudience value: {v}.");
        }
        return list;
    }

    private static List<int> ValidateTopics(List<int>? list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("Topics is required and must contain at least one value.");
        var valid = Enum.GetValues<EventTopic>().Select(e => (int)e).ToHashSet();
        foreach (var v in list)
        {
            if (!valid.Contains(v))
                throw new ArgumentException($"Invalid Topics value: {v}.");
        }
        return list;
    }

    private static EventDto ToDto(Event e) =>
        new(e.Id, e.Title, e.Description, e.EventLink,
            e.TargetAudience, e.Topics,
            e.DateStart, e.DateEnd,
            e.Classification.ToString(),
            e.Fees, e.IsOnline,
            e.OrganizationId,
            e.ProgramName, e.Format,
            e.SchoolBookable, e.AgeMinimum, e.AgeMaximum,
            e.LocationId, e.ContactId,
            e.Status.ToString(),
            e.CreatedBy, e.ModifiedBy,
            e.CreatedAt, e.ModifiedAt);
}
