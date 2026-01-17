using AutoMapper;
using DNAustria.Backend.Data;
using DNAustria.Backend.Dtos;
using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Backend.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILLMService _llmService;

    public EventService(AppDbContext db, IMapper mapper, ILLMService llmService)
    {
        _db = db;
        _mapper = mapper;
        _llmService = llmService;
    }

    public async Task<IEnumerable<EventListDto>> GetEventsAsync(EventStatus? status = null, string? q = null)
    {
        var query = _db.Events.AsQueryable();
        if (status.HasValue) query = query.Where(e => e.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(e => e.Title.Contains(q) || e.Description.Contains(q));
        }
        return await query.Select(e => new EventListDto(e.Id, e.Title, e.DateStart, e.DateEnd, e.Status)).ToListAsync();
    }

    public async Task<EventDetailDto?> GetEventAsync(Guid id)
    {
        var e = await _db.Events.Include(x => x.Address).Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return null;
        return _mapper.Map<EventDetailDto>(e);
    }

    public async Task<EventDetailDto> CreateEventAsync(EventCreateDto dto)
    {
        var ev = _mapper.Map<Event>(dto);
        ev.Id = Guid.NewGuid();
        ev.ModifiedAt = DateTime.UtcNow;

        // Address: reuse if LocationId provided, else try matching by Zip + Lat/Lon, else create
        if (dto.LocationId.HasValue)
        {
            var existing = await _db.Addresses.FindAsync(dto.LocationId.Value);
            if (existing != null) { ev.Address = existing; ev.LocationId = existing.Id; }
        }
        else if (dto.Address != null)
        {
            var a = dto.Address;
            var existing = await _db.Addresses.FirstOrDefaultAsync(x => x.Zip == a.Zip && x.Latitude == a.Latitude && x.Longitude == a.Longitude);
            if (existing != null) { ev.Address = existing; ev.LocationId = existing.Id; }
            else
            {
                var newAddr = new Models.Address
                {
                    Id = Guid.NewGuid(),
                    LocationName = a.LocationName,
                    City = a.City,
                    Zip = a.Zip,
                    State = a.State,
                    Street = a.Street,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude
                };
                _db.Addresses.Add(newAddr);
                await _db.SaveChangesAsync();
                ev.Address = newAddr; ev.LocationId = newAddr.Id;
            }
        }

        // Contact: reuse if provided id, else create inline
        if (dto.ContactId.HasValue)
        {
            var contact = await _db.Contacts.FindAsync(dto.ContactId.Value);
            if (contact != null) { ev.Contact = contact; ev.ContactId = contact.Id; }
        }
        else if (dto.Contact != null)
        {
            var c = dto.Contact;
            var newC = new Models.Contact { Id = Guid.NewGuid(), Name = c.Name, Org = c.Org, Email = c.Email, Phone = c.Phone };
            _db.Contacts.Add(newC);
            await _db.SaveChangesAsync();
            ev.Contact = newC; ev.ContactId = newC.Id;
        }

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return _mapper.Map<EventDetailDto>(ev);
    }

    public async Task<EventDetailDto?> UpdateEventAsync(Guid id, EventCreateDto dto)
    {
        var e = await _db.Events.Include(x => x.Address).Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return null;
        if (e.Status == EventStatus.Transferred) return null;

        // map primitive fields
        _mapper.Map(dto, e);
        e.ModifiedAt = DateTime.UtcNow;

        // Address logic: LocationId has precedence; else if Address provided then try reuse or create
        if (dto.LocationId.HasValue)
        {
            var a = await _db.Addresses.FindAsync(dto.LocationId.Value);
            if (a != null) { e.Address = a; e.LocationId = a.Id; }
        }
        else if (dto.Address != null)
        {
            var a = dto.Address;
            var existing = await _db.Addresses.FirstOrDefaultAsync(x => x.Zip == a.Zip && x.Latitude == a.Latitude && x.Longitude == a.Longitude);
            if (existing != null) { e.Address = existing; e.LocationId = existing.Id; }
            else
            {
                var newAddr = new Models.Address
                {
                    Id = Guid.NewGuid(),
                    LocationName = a.LocationName,
                    City = a.City,
                    Zip = a.Zip,
                    State = a.State,
                    Street = a.Street,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude
                };
                _db.Addresses.Add(newAddr);
                await _db.SaveChangesAsync();
                e.Address = newAddr; e.LocationId = newAddr.Id;
            }
        }

        // Contact logic
        if (dto.ContactId.HasValue)
        {
            var c = await _db.Contacts.FindAsync(dto.ContactId.Value);
            if (c != null) { e.Contact = c; e.ContactId = c.Id; }
        }
        else if (dto.Contact != null)
        {
            var c = dto.Contact;
            var newC = new Models.Contact { Id = Guid.NewGuid(), Name = c.Name, Org = c.Org, Email = c.Email, Phone = c.Phone };
            _db.Contacts.Add(newC);
            await _db.SaveChangesAsync();
            e.Contact = newC; e.ContactId = newC.Id;
        }

        await _db.SaveChangesAsync();
        return _mapper.Map<EventDetailDto>(e);
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var e = await _db.Events.FindAsync(id);
        if (e is null) return false;
        _db.Events.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, EventStatus status)
    {
        var e = await _db.Events.FindAsync(id);
        if (e is null) return false;
        if (e.Status == EventStatus.Transferred) return false;
        e.Status = status;
        e.ModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<EventCreateDto> ParseEventAsync(string input, bool isHtml)
    {
        // Use LLM service to parse unstructured input into EventCreateDto
        var parsed = await _llmService.ParseEventAsync(input, isHtml);
        return parsed;
    }

    public async Task<Dtos.ExportEventsResultDto> GetApprovedEventsExportAsync()
    {
        var events = await _db.Events
            .Include(e => e.Address)
            .Include(e => e.Contact)
            .Where(e => e.Status == EventStatus.Approved)
            .ToListAsync();

        var orgs = await _db.Organizations.ToDictionaryAsync(o => o.Id, o => o.Name);

        var exported = new Dtos.ExportEventsResultDto();

        foreach (var e in events)
        {
            // Ensure event_link is a valid absolute http(s) URL. Use provided link if valid; otherwise fall back to a canonical event URL.
            string link;
            if (!string.IsNullOrWhiteSpace(e.EventLink) && Uri.TryCreate(e.EventLink, UriKind.Absolute, out var parsed) && (parsed.Scheme == "http" || parsed.Scheme == "https"))
            {
                link = e.EventLink!;
            }
            else
            {
                // Fallback to an API URL that points to the event detail. This is intentionally HTTP to avoid requiring HTTPS in all environments.
                link = $"http://localhost:5000/api/events/{e.Id}";
            }

            var dto = new Dtos.ExportEventDto
            {
                Event_Title = e.Title,
                Event_Description = e.Description,
                Event_Link = link,
                Event_Target_Audience = e.TargetAudience?.Select(ta => (int)ta).ToList() ?? new List<int>(),
                Event_Topics = e.Topics?.Select(t => (int)t).ToList() ?? new List<int>(),
                Event_Start = e.DateStart.ToString("o"),
                Event_End = e.DateEnd.ToString("o"),
                Event_Classification = e.Classification == Classification.Scheduled ? "scheduled" : "on-demand",
                Event_Has_Fees = e.Fees,
                Event_Is_Online = e.IsOnline,
                Organization_Name = e.OrganizationId.HasValue && orgs.TryGetValue(e.OrganizationId.Value, out var on) ? on : string.Empty,

                Program_Name = e.ProgramName,
                Event_Format = e.Format,
                Event_School_Bookable = e.SchoolBookable,
                Event_Age_Minimum = e.AgeMinimum,
                Event_Age_Maximum = e.AgeMaximum,

                Event_Location_Name = e.Address?.LocationName,
                Event_Address_Street = e.Address?.Street,
                Event_Address_City = e.Address?.City,
                Event_Address_Zip = e.Address?.Zip,
                Event_Address_State = NormalizeState(e.Address?.State),

                Event_Contact_Name = e.Contact?.Name,
                Event_Contact_Org = e.Contact?.Org,
                Event_Contact_Email = e.Contact?.Email,
                Event_Contact_Phone = e.Contact?.Phone,

                Location = (e.Address?.Latitude.HasValue == true && e.Address?.Longitude.HasValue == true) ? new List<double> { e.Address.Latitude!.Value, e.Address.Longitude!.Value } : null,
                Group_Id = null
            };

            exported.Events.Add(dto);
        }

        return exported;
    }

    private string? NormalizeState(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var cleaned = raw.Trim();
        // Allowed states (as required by the export schema)
        var allowed = new[] { "Burgenland","Kärnten","Niederösterreich","Oberösterreich","Salzburg","Steiermark","Tirol","Vorarlberg","Wien" };
        // Try exact match first
        if (allowed.Contains(cleaned, StringComparer.OrdinalIgnoreCase))
            return allowed.First(s => string.Equals(s, cleaned, StringComparison.OrdinalIgnoreCase));

        // Try a small mapping for common alternatives (lowercase, abbreviations, english names)
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "styria", "Steiermark" },
            { "tyrol", "Tirol" },
            { "carinthia", "Kärnten" },
            { "lower austria", "Niederösterreich" },
            { "upper austria", "Oberösterreich" },
            { "salzburg", "Salzburg" },
            { "vienna", "Wien" }
        };
        if (map.TryGetValue(cleaned, out var mapped)) return mapped;

        return null; // don't include invalid values
    }
}