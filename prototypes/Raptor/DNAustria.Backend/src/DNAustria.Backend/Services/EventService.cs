using System;
using System.Linq;
using AutoMapper;
using DNAustria.Backend.Data;
using DNAustria.Backend.Dtos;
using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DNAustria.Backend.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILLMService _llmService;
    private readonly string _externalBaseUrl;

    public EventService(AppDbContext db, IMapper mapper, ILLMService llmService, IConfiguration? config)
    {
        _db = db;
        _mapper = mapper;
        _llmService = llmService;

        // Determine external base URL for generating event links.
        // Priority: EXTERNAL_BASE_URL env var, then ASPNETCORE_URLS env var, then configuration key "ExternalBaseUrl", fallback to http://localhost:5000
        var external = Environment.GetEnvironmentVariable("EXTERNAL_BASE_URL") ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? config?["ExternalBaseUrl"];
        if (!string.IsNullOrEmpty(external))
        {
            var candidate = external.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => s.Trim())
                                     .FirstOrDefault(s => s.StartsWith("http://") || s.StartsWith("https://"));
            if (!string.IsNullOrEmpty(candidate))
            {
                try
                {
                    var uri = new Uri(candidate);
                    var host = uri.Host;
                    if (host == "+" || host == "0.0.0.0") host = "localhost";
                    var ub = new UriBuilder(uri) { Host = host };
                    _externalBaseUrl = ub.Uri.ToString().TrimEnd('/');
                }
                catch
                {
                    _externalBaseUrl = candidate.Replace("0.0.0.0", "localhost").Replace("+", "localhost").TrimEnd('/');
                }
            }
            else
            {
                _externalBaseUrl = string.Empty;
            }
        }
        else
        {
            _externalBaseUrl = string.Empty;
        }

        if (string.IsNullOrEmpty(_externalBaseUrl)) _externalBaseUrl = "http://localhost:5000";
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
        var e = await _db.Events.Include(x => x.Location).Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return null;
        return _mapper.Map<EventDetailDto>(e);
    }

    public async Task<EventDetailDto> CreateEventAsync(EventCreateDto dto)
    {
        var ev = _mapper.Map<Event>(dto);
        ev.Id = Guid.NewGuid();
        ev.ModifiedAt = DateTime.UtcNow;

        // Ensure any mapped navigation is cleared so EF won't create an Organization entity from the DTO by accident.
        ev.Location = null;

        // Location (Organization) logic: LocationId has precedence. Inline location fields are saved on the Event when provided.
        if (dto.LocationId.HasValue)
        {
            var existing = await _db.Organizations.FindAsync(dto.LocationId.Value);
            if (existing != null)
            {
                ev.Location = existing;
                ev.LocationId = existing.Id;
                // also copy organization fields into event inline fields for export convenience
                ev.LocationName = existing.Name;
                ev.LocationStreet = existing.Street;
                ev.LocationCity = existing.City;
                ev.LocationZip = existing.Zip;
                ev.LocationState = existing.State;
                ev.LocationLatitude = existing.Latitude;
                ev.LocationLongitude = existing.Longitude;
            }
        }
        else if (dto.Location != null)
        {
            var a = dto.Location;
            var existing = await _db.Organizations.FirstOrDefaultAsync(x => x.Zip == a.Zip && x.Latitude == a.Latitude && x.Longitude == a.Longitude);
            if (existing != null)
            {
                ev.Location = existing;
                ev.LocationId = existing.Id;
                ev.LocationName = existing.Name;
                ev.LocationStreet = existing.Street;
                ev.LocationCity = existing.City;
                ev.LocationZip = existing.Zip;
                ev.LocationState = existing.State;
                ev.LocationLatitude = existing.Latitude;
                ev.LocationLongitude = existing.Longitude;
            }
            else
            {
                // store inline organization fields on the event (do NOT create an Organization)
                ev.LocationName = a.Name;
                ev.LocationStreet = a.Street;
                ev.LocationCity = a.City;
                ev.LocationZip = a.Zip;
                ev.LocationState = a.State;
                ev.LocationLatitude = a.Latitude;
                ev.LocationLongitude = a.Longitude;
                ev.LocationId = null;
                ev.Location = null;
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
        var e = await _db.Events.Include(x => x.Location).Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return null;
        if (e.Status == EventStatus.Transferred) return null;

        // map primitive fields
        _mapper.Map(dto, e);
        e.ModifiedAt = DateTime.UtcNow;

        // Location (Organization) logic: LocationId has precedence.
        // Inline location fields are saved on the Event when provided. Organization is used only for selection.
        if (dto.LocationId.HasValue)
        {
            var a = await _db.Organizations.FindAsync(dto.LocationId.Value);
            if (a != null)
            {
                e.Location = a;
                e.LocationId = a.Id;
                e.LocationName = a.Name;
                e.LocationStreet = a.Street;
                e.LocationCity = a.City;
                e.LocationZip = a.Zip;
                e.LocationState = a.State;
                e.LocationLatitude = a.Latitude;
                e.LocationLongitude = a.Longitude;
            }
        }
        else
        {
            // Prefer explicit inline fields from the DTO if provided
            if (!string.IsNullOrWhiteSpace(dto.LocationName) || !string.IsNullOrWhiteSpace(dto.LocationCity) || !string.IsNullOrWhiteSpace(dto.LocationZip))
            {
                e.LocationName = dto.LocationName;
                e.LocationStreet = dto.LocationStreet;
                e.LocationCity = dto.LocationCity;
                e.LocationZip = dto.LocationZip;
                e.LocationState = dto.LocationState;
                e.LocationLatitude = dto.LocationLatitude;
                e.LocationLongitude = dto.LocationLongitude;
                // Clear any previous LocationId/reference because inline fields are being used
                e.LocationId = null;
                e.Location = null;
            }
            else if (dto.Location != null)
            {
                var a = dto.Location;
                var existing = await _db.Organizations.FirstOrDefaultAsync(x => x.Zip == a.Zip && x.Latitude == a.Latitude && x.Longitude == a.Longitude);
                if (existing != null)
                {
                    e.Location = existing;
                    e.LocationId = existing.Id;
                    e.LocationName = existing.Name;
                    e.LocationStreet = existing.Street;
                    e.LocationCity = existing.City;
                    e.LocationZip = existing.Zip;
                    e.LocationState = existing.State;
                    e.LocationLatitude = existing.Latitude;
                    e.LocationLongitude = existing.Longitude;
                }
                else
                {
                    // store inline organization fields on the event (do NOT create an Organization)
                    e.LocationName = a.Name;
                    e.LocationStreet = a.Street;
                    e.LocationCity = a.City;
                    e.LocationZip = a.Zip;
                    e.LocationState = a.State;
                    e.LocationLatitude = a.Latitude;
                    e.LocationLongitude = a.Longitude;
                    e.LocationId = null;
                    e.Location = null;
                }
            }
            // If no inline fields provided, keep existing event location fields as they are
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
        // Load event *columns only* (projection) to ensure EF does not generate joins to removed tables.
        var events = await _db.Events
            .Where(e => e.Status == EventStatus.Approved)
            .Select(e => new
            {
                e.Id,
                e.Title,
                e.Description,
                e.EventLink,
                e.TargetAudience,
                e.Topics,
                e.DateStart,
                e.DateEnd,
                e.Classification,
                e.Fees,
                e.IsOnline,
                e.OrganizationId,
                e.ProgramName,
                e.Format,
                e.SchoolBookable,
                e.AgeMinimum,
                e.AgeMaximum,
                e.LocationId,
                e.LocationName,
                e.LocationStreet,
                e.LocationCity,
                e.LocationZip,
                e.LocationState,
                e.LocationLatitude,
                e.LocationLongitude,
                e.ContactId
            })
            .ToListAsync();

        // Load referenced organizations and contacts separately to avoid any navigation joins
        var orgIds = events.Where(e => e.LocationId.HasValue).Select(e => e.LocationId!.Value).Distinct().ToList();
        var orgs = await _db.Organizations.Where(o => orgIds.Contains(o.Id)).ToDictionaryAsync(o => o.Id, o => o);

        var contactIds = events.Where(e => e.ContactId.HasValue).Select(e => e.ContactId!.Value).Distinct().ToList();
        var contacts = await _db.Contacts.Where(c => contactIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, c => c);

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
                link = $"{_externalBaseUrl}/api/events/{e.Id}";
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
                Organization_Name = e.OrganizationId.HasValue && orgs.TryGetValue(e.OrganizationId.Value, out var on) ? on.Name : string.Empty,

                Program_Name = e.ProgramName,
                Event_Format = e.Format,
                Event_School_Bookable = e.SchoolBookable,
                Event_Age_Minimum = e.AgeMinimum,
                Event_Age_Maximum = e.AgeMaximum,

                // Prefer inline event location fields when present, otherwise lookup selected Organization
                Event_Location_Name = !string.IsNullOrWhiteSpace(e.LocationName) ? e.LocationName : (e.LocationId.HasValue && orgs.TryGetValue(e.LocationId.Value, out var orgVal) ? orgVal.Name : null),
                Event_Address_Street = !string.IsNullOrWhiteSpace(e.LocationStreet) ? e.LocationStreet : (e.LocationId.HasValue && orgs.TryGetValue(e.LocationId.Value, out var orgVal2) ? orgVal2.Street : null),
                Event_Address_City = !string.IsNullOrWhiteSpace(e.LocationCity) ? e.LocationCity : (e.LocationId.HasValue && orgs.TryGetValue(e.LocationId.Value, out var orgVal3) ? orgVal3.City : null),
                Event_Address_Zip = !string.IsNullOrWhiteSpace(e.LocationZip) ? e.LocationZip : (e.LocationId.HasValue && orgs.TryGetValue(e.LocationId.Value, out var orgVal4) ? orgVal4.Zip : null),
                Event_Address_State = NormalizeState(!string.IsNullOrWhiteSpace(e.LocationState) ? e.LocationState : (e.LocationId.HasValue && orgs.TryGetValue(e.LocationId.Value, out var orgVal5) ? orgVal5.State : null)),


                Event_Contact_Name = e.ContactId.HasValue && contacts.TryGetValue(e.ContactId.Value, out var cval) ? cval.Name : null,
                Event_Contact_Org = e.ContactId.HasValue && contacts.TryGetValue(e.ContactId.Value, out var cval2) ? cval2.Org : null,
                Event_Contact_Email = e.ContactId.HasValue && contacts.TryGetValue(e.ContactId.Value, out var cval3) ? cval3.Email : null,
                Event_Contact_Phone = e.ContactId.HasValue && contacts.TryGetValue(e.ContactId.Value, out var cval4) ? cval4.Phone : null,

                Location = (e.LocationLatitude.HasValue && e.LocationLongitude.HasValue) ? new List<double> { e.LocationLatitude.Value, e.LocationLongitude.Value }
                          : (e.LocationId.HasValue && orgs.TryGetValue(e.LocationId.Value, out var orgValLoc) && orgValLoc.Latitude.HasValue && orgValLoc.Longitude.HasValue ? new List<double> { orgValLoc.Latitude.Value, orgValLoc.Longitude.Value } : null),
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