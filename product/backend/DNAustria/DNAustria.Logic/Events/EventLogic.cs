using DNAustria.Dal.Data;
using DNAustria.Domain;
using Microsoft.EntityFrameworkCore;

using DomainEvent = DNAustria.Domain.Event;
using DalEvent = DNAustria.Dal.Models.Event;

namespace DNAustria.Logic.Events;


public class EventLogic : IEventLogic
{
    private readonly AppDbContext _db;

    private const EventStatus PublicStatus = EventStatus.Published;

    public EventLogic(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DomainEvent>> GetAllAsync(string? name)
    {
        var query = _db.Events
            .AsNoTracking()
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(e => EF.Functions.ILike(e.Name, $"%{name}%"));

        var entities = await query
            .OrderBy(e => e.StartDate)
            .ToListAsync();

        return entities.Select(MapToDomain).ToList();
    }

    public async Task<DomainEvent?> GetByIdAsync(int id)
    {
        var entity = await _db.Events
            .AsNoTracking()
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences)
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null ? null : MapToDomain(entity);
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

        _db.Events.Add(entity);
        await _db.SaveChangesAsync();

        return MapToDomain(entity);
    }

    public async Task<DomainEvent?> UpdateAsync(
        int id,
        DomainEvent domain,
        IEnumerable<int>? targetAudiences,
        IEnumerable<int>? topics)
    {
        var entity = await _db.Events
            .Include(e => e.EventTopics)
            .Include(e => e.EventTargetAudiences)
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

        await _db.SaveChangesAsync();

        return MapToDomain(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Events.FindAsync(new object[] { id });
        if (entity is null)
            return false;

        _db.Events.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<DomainEvent?> UpdateStatusAsync(int id, EventStatus status)
    {
        var entity = await _db.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
            return null;

        entity.Status = (int)status;

        await _db.SaveChangesAsync();

        return MapToDomain(entity);
    }
    
    private static DomainEvent MapToDomain(DalEvent e)
    {
        var domain = new DomainEvent(
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
            e.EventTopics.Select(x => x.Topic));

        return domain;
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

            // Domain Enum -> DAL int
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