using DNAustria.Dal.Data;
using DNAustria.Domain;
using Microsoft.EntityFrameworkCore;

using DomainEvent = DNAustria.Domain.Event;
using DalEvent = DNAustria.Dal.Models.Event;

namespace DNAustria.Logic.Events;


public class EventLogic (AppDbContext db, IEventTracker tracker) : IEventLogic
{
    public async Task<PagedResult<DomainEvent>> GetAllAsync(string? name, EventStatus? status, int page, int pageSize)
    {
        var normalizedPage = Math.Max(1, page);
        var normalizedPageSize = Math.Max(1, pageSize);

        var query = db.Events
            .AsNoTracking()
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences)
            .Where(e => !e.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{name}%"));
        }

        if (status is not null)
        {
            query = query.Where(e => e.Status == (int)status);
        }

        var totalCount = await query.CountAsync();
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)normalizedPageSize);

        var entities = await query
            .OrderBy(e => e.StartDate)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync();

        return new PagedResult<DomainEvent>
        {
            Items = entities.Select(MapToDomain).ToList(),
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<DomainEvent?> GetByIdAsync(int id)
    {
        var entity = await db.Events
            .AsNoTracking()
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences).Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<IReadOnlyList<EventHistoryEntry>> GetHistoryByEventIdAsync(int id)
    {
        return await db.EventHistories
            .AsNoTracking()
            .Where(h => h.EventId == id)
            .Include(h => h.User)
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => new EventHistoryEntry
            {
                Action = h.Action,
                CreatedAt = h.CreatedAt,
                Username = h.User.Username
            })
            .ToListAsync();
    }

    public async Task<DomainEvent> CreateAsync(
        DomainEvent domain,
        IEnumerable<int>? targetAudiences,
        IEnumerable<int>? topics)
    {
        var entity = MapToDal(domain);

        entity.EventTargetAudiences = (targetAudiences ?? Enumerable.Empty<int>())
            .Distinct()
            .Select(x => new Dal.Models.EventTargetAudience
            {
                TargetAudience = x
            }).ToList();

        entity.EventTopics = (topics ?? Enumerable.Empty<int>())
            .Distinct()
            .Select(x => new Dal.Models.EventTopic
            {
                Topic = x
            }).ToList();

        db.Events.Add(entity);
        await db.SaveChangesAsync();
        
        await tracker.TrackAsync(
            entity.Id,
            118811,
            $"Event created");

        return MapToDomain(entity);
    }

    public async Task<DomainEvent?> UpdateAsync(
        int id,
        DomainEvent domain,
        IEnumerable<int>? targetAudiences,
        IEnumerable<int>? topics)
    {
        var entity = await db.Events
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences).Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is null)
            return null;

        UpdateDalEntity(entity, domain);

        entity.EventTargetAudiences.Clear();
        entity.EventTopics.Clear();

        foreach (var ta in (targetAudiences ?? Enumerable.Empty<int>()).Distinct())
        {
            entity.EventTargetAudiences.Add(new Dal.Models.EventTargetAudience
            {
                Event = entity.Id,
                TargetAudience = ta
            });
        }

        foreach (var t in (topics ?? Enumerable.Empty<int>()).Distinct())
        {
            entity.EventTopics.Add(new Dal.Models.EventTopic
            {
                Event = entity.Id,
                Topic = t
            });
        }

        await db.SaveChangesAsync();
        
        await tracker.TrackAsync(
            entity.Id,
            118811,
            $"Event updated");

        return MapToDomain(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Events.FindAsync(new object[] { id });
        if (entity is null)
            return false;

        entity.IsDeleted = true;
        await db.SaveChangesAsync();
        
        await tracker.TrackAsync(
            entity.Id,
            118811,
            $"Event deleted");
        return true;
    }

    public async Task<DomainEvent?> UpdateStatusAsync(int id, EventStatus status)
    {
        var entity = await db.Events.Where(e => !e.IsDeleted).FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
            return null;

        entity.Status = (int)status;

        await db.SaveChangesAsync();
        
        await tracker.TrackAsync(
            entity.Id,
            118811,
            $"Event Status set to {status}");

        return MapToDomain(entity);
    }


    public async Task<IReadOnlyList<DalEvent>> HandlePublishEventsAsync()
    {
        var events = await db.Events
            .Where(e => !e.IsDeleted && e.Status != (int)EventStatus.Draft)
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences)
            .Include(e => e.OrganizationNavigation)
            .Include(e => e.LocationNavigation)
                .ThenInclude(l => l!.AddressNavigation)
            .Include(e => e.ContactNavigation)
            .OrderBy(e => e.StartDate)
            .ToListAsync();

        foreach (var e in events)
        {
            e.Status = (int)EventStatus.Published;
        }

        await db.SaveChangesAsync();

        return events;
    }

    private static DomainEvent MapToDomain(DalEvent e)
    {
        return DomainEvent.Rehydrate(
            e.Id,
            e.Name,
            e.Description,
            e.Link,
            e.StartDate,
            e.EndDate,
            (EventClassification)e.Classification,
            (EventStatus)e.Status,
            e.HasFees,
            e.IsOnline,
            e.ProgramName,
            e.Format,
            e.SchoolBookable,
            e.AgeMinimum,
            e.AgeMaximum,
            e.Organization,
            e.Location,
            e.Contact,
            e.EventTargetAudiences.Select(x => x.TargetAudience),
            e.EventTopics.Select(x => x.Topic)
        );
    }

    private static DalEvent MapToDal(DomainEvent d)
    {
        return new DalEvent
        {
            Name = d.Name,
            Description = d.Description,
            Link = d.Link,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            
            Classification = (int)d.Classification,
            Status = (int)d.Status,

            HasFees = d.HasFees,
            IsOnline = d.IsOnline,
            
            Organization = d.OrganizationId,
            Location = d.LocationId,
            Contact = d.ContactId,

            ProgramName = d.ProgramName,
            Format = d.Format,
            SchoolBookable = d.SchoolBookable,
            AgeMinimum = d.AgeMinimum,
            AgeMaximum = d.AgeMaximum,
        };
    }

    private static void UpdateDalEntity(DalEvent entity, DomainEvent domain)
    {
        entity.Name = domain.Name;
        entity.Description = domain.Description;
        entity.Link = domain.Link;
        entity.StartDate = domain.StartDate;
        entity.EndDate = domain.EndDate;
        
        entity.Classification = (int)domain.Classification;
        entity.Status = (int)domain.Status;

        entity.HasFees = domain.HasFees;
        entity.IsOnline = domain.IsOnline;
        
        entity.Organization = domain.OrganizationId;
        entity.Location = domain.LocationId;
        entity.Contact = domain.ContactId;

        entity.ProgramName = domain.ProgramName;
        entity.Format = domain.Format;
        entity.SchoolBookable = domain.SchoolBookable;
        entity.AgeMinimum = domain.AgeMinimum;
        entity.AgeMaximum = domain.AgeMaximum;
    }
}
