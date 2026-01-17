using DNAustria.API.Data;
using DNAustria.API.Models;
using DNAustria.API.Models.Dtos;
using DNAustria.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AddressService _addressService;
    private readonly ContactService _contactService;

    public EventsController(AppDbContext context, AddressService addressService, ContactService contactService)
    {
        _context = context;
        _addressService = addressService;
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<EventListResponse>> GetEvents([FromQuery] string? search, [FromQuery] EventStatus? status)
    {
        var query = _context.Events
            .Include(e => e.Location)
            .Include(e => e.Contact)
            .Include(e => e.Organization)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(e => e.Title.ToLower().Contains(search) || 
                                     e.Description.ToLower().Contains(search));
        }

        var events = await query.ToListAsync();

        var dtos = events.Select(e => new EventExportDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            EventLink = !string.IsNullOrEmpty(e.EventLink) && (e.EventLink.StartsWith("http://") || e.EventLink.StartsWith("https://")) 
                ? e.EventLink 
                : "https://www.dnaustria.at", // Default to a valid URL if missing or invalid
            TargetAudience = e.TargetAudience.Select(a => (int)a).ToList(),
            Topics = e.Topics.Select(t => (int)t).ToList(),
            DateStart = e.DateStart,
            DateEnd = e.DateEnd,
            Classification = e.Classification.ToString().ToLower(),
            Fees = e.Fees,
            IsOnline = e.IsOnline,
            OrganizationName = e.Organization?.Name ?? "Unknown Organization",
            ProgramName = e.ProgramName ?? "Default Program",
            Format = e.Format ?? "Default Format",
            SchoolBookable = e.SchoolBookable ?? false,
            AgeMinimum = e.AgeMinimum ?? 0,
            AgeMaximum = e.AgeMaximum ?? 99,
            LocationName = e.Location?.LocationName,
            Street = e.Location?.Street,
            City = e.Location?.City,
            Zip = e.Location?.Zip,
            State = ValidateState(e.Location?.State),
            MintRegion = 10,
            ContactName = e.Contact?.Name,
            ContactOrg = e.Contact?.Org,
            ContactEmail = e.Contact?.Email,
            ContactPhone = e.Contact?.Phone,
            LocationCoordinates = e.Location != null 
                ? new[] { e.Location.Latitude, e.Location.Longitude } 
                : new double[] { }
        }).ToList();

        return new EventListResponse { Events = dtos };
    }

    private string? ValidateState(string? state)
    {
        var validStates = new[] { "Burgenland", "Kärnten", "Niederösterreich", "Oberösterreich", "Salzburg", "Steiermark", "Tirol", "Vorarlberg", "Wien" };
        if (state != null && validStates.Contains(state)) return state;
        return "Wien"; // Default fallback
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> GetEvent(Guid id)
    {
        var evt = await _context.Events
            .Include(e => e.Location)
            .Include(e => e.Contact)
            .Include(e => e.Organization)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evt == null)
        {
            return NotFound();
        }

        return evt;
    }

    [HttpPost]
    public async Task<ActionResult<Event>> CreateEvent(Event evt)
    {
        // Validate Organization
        if (evt.OrganizationId == Guid.Empty)
        {
             return BadRequest("Organization is required.");
        }
        
        var orgExists = await _context.Organizations.AnyAsync(o => o.Id == evt.OrganizationId);
        if (!orgExists)
        {
             return BadRequest($"Organization with ID {evt.OrganizationId} does not exist.");
        }

        // Handle Address Reuse
        if (evt.Location != null)
        {
            var address = await _addressService.GetOrCreateAddressAsync(evt.Location);
            evt.LocationId = address.Id;
            evt.Location = null;
        }

        // Handle Contact Reuse
        if (evt.Contact != null)
        {
            var contact = await _contactService.GetOrCreateContactAsync(evt.Contact);
            evt.ContactId = contact.Id;
            evt.Contact = null;
        }

        evt.Id = Guid.NewGuid();
        evt.ModifiedAt = DateTime.UtcNow;
        
        _context.Events.Add(evt);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = evt.Id }, evt);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, Event evt)
    {
        if (id != evt.Id)
        {
            return BadRequest();
        }

        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent == null)
        {
            return NotFound();
        }

        if (existingEvent.Status == EventStatus.Transferred)
        {
            return BadRequest("Cannot edit transferred events.");
        }

        // Handle Address Reuse
        if (evt.Location != null)
        {
            var address = await _addressService.GetOrCreateAddressAsync(evt.Location);
            evt.LocationId = address.Id;
            evt.Location = null;
        }

        // Handle Contact Reuse
        if (evt.Contact != null)
        {
            var contact = await _contactService.GetOrCreateContactAsync(evt.Contact);
            evt.ContactId = contact.Id;
            evt.Contact = null;
        }

        // Update properties
        _context.Entry(existingEvent).CurrentValues.SetValues(evt);
        existingEvent.ModifiedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EventExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
        {
            return NotFound();
        }

        _context.Events.Remove(evt);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EventExists(Guid id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}
