using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class EventService(IAppDbContext ctx, IMapper mapper) : IEventService
{
    public async Task<IEnumerable<EventDto>> ListAsync(string? search, string? filter, int page, int pageSize)
    {
        var query = ctx.Events.Include(e=>e.Organization).Include(e=>e.Contact).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Title.ToLower().Contains(search.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(filter))
        {
            // einfache Filter-Syntax: status:Approved | is_online:true
            var parts = filter.Split(':',2);
            if(parts.Length==2){
                var key = parts[0].Trim().ToLower(); var val = parts[1].Trim();
                query = key switch
                {
                    "status" when Enum.TryParse<EventStatus>(val, true, out var st) => query.Where(e => e.Status == st),
                    "is_online" when bool.TryParse(val, out var io) => query.Where(e => e.IsOnline == io),
                    _ => query
                };
            }
        }
        var items = await query.OrderByDescending(e=>e.ModifiedAt)
            .Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
        return (mapper.Map<IEnumerable<EventDto>>(items));
    }

    public async Task<EventDto> CreateAsync(EventDto dto)
    {
        var entity = mapper.Map<Event>(dto);
        entity.Id = Guid.NewGuid();
        entity.ModifiedAt = DateTime.UtcNow;
        entity.CreatedBy = "system";
        entity.ModifiedBy = "system";
        ctx.Events.Add(entity);
        await ctx.SaveChangesAsync();
        return mapper.Map<EventDto>(entity);
    }

    public async Task<EventDto?> UpdateAsync(Guid id, EventDto dto)
    {
        var entity = await ctx.Events.FirstOrDefaultAsync(e=>e.Id==id);
        if (entity == null) return null;
        if (entity.Status == EventStatus.Transferred) return null; // Regel: nur Draft/Approved
        mapper.Map(dto, entity);
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = "system";
        await ctx.SaveChangesAsync();
        return mapper.Map<EventDto>(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await ctx.Events.FirstOrDefaultAsync(e=>e.Id==id);
        if (entity == null) return false;
        ctx.Events.Remove(entity);
        await ctx.SaveChangesAsync();
        return true;
    }

    public async Task<EventDto?> ImportAsync(string raw)
    {
        // Sehr einfache Heuristik: erste Zeile Titel, rest Beschreibung
        var lines = raw.Split('\n');
        var title = lines.FirstOrDefault()?.Trim() ?? "Imported Event";
        var description = string.Join("\n", lines.Skip(1)).Trim();
        var entity = new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Status = EventStatus.Draft,
            ModifiedAt = DateTime.UtcNow
        };
        ctx.Events.Add(entity);
        await ctx.SaveChangesAsync();
        return mapper.Map<EventDto>(entity);
    }

    public async Task<IEnumerable<ExportEventDto>> PublicExportAsync()
    {
        var events = await ctx.Events
            .Include(e=>e.Organization)
            .Include(e=>e.Contact)
            .Where(e => e.Status == EventStatus.Approved || e.Status == EventStatus.Transferred)
            .ToListAsync();
        return mapper.Map<IEnumerable<ExportEventDto>>(events);
    }

    public async Task<EventDto?> GetByIdAsync(Guid id)
    {
        var entity = await ctx.Events.FirstOrDefaultAsync(e=>e.Id==id);
        return entity == null ? null : mapper.Map<EventDto>(entity);
    }
}
