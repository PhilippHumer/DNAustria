using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class EventImportService : IEventImportService
{
    private readonly IAppDbContext _ctx;

    public EventImportService(IAppDbContext ctx)
    {
        _ctx = ctx;
    }

    private class ImportModel
    {
        [JsonPropertyName("event_title")] public string EventTitle { get; set; } = string.Empty;
        [JsonPropertyName("event_description")] public string? EventDescription { get; set; }
        [JsonPropertyName("event_start")] public DateTime? EventStart { get; set; }
        [JsonPropertyName("event_end")] public DateTime? EventEnd { get; set; }
        [JsonPropertyName("event_link")] public string? EventLink { get; set; }
        [JsonPropertyName("event_topics")] public int[]? EventTopics { get; set; }
        [JsonPropertyName("event_target_audience")] public int[]? EventTargetAudience { get; set; }
        [JsonPropertyName("event_is_online")] public bool EventIsOnline { get; set; }
        [JsonPropertyName("organization_name")] public string? OrganizationName { get; set; }
        [JsonPropertyName("event_contact_email")] public string? EventContactEmail { get; set; }
        [JsonPropertyName("event_contact_phone")] public string? EventContactPhone { get; set; }
        [JsonPropertyName("event_address_street")] public string? EventAddressStreet { get; set; }
        [JsonPropertyName("event_address_city")] public string? EventAddressCity { get; set; }
        [JsonPropertyName("event_address_zip")] public string? EventAddressZip { get; set; }
        [JsonPropertyName("event_address_state")] public string? EventAddressState { get; set; }
    }

    public async Task<int> ImportAsync(string json, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(json)) return 0;
        ImportModel[]? items;
        try
        {
            items = JsonSerializer.Deserialize<ImportModel[]>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return 0; // ungültiges JSON
        }
        if (items == null || items.Length == 0) return 0;

        int created = 0;
        foreach (var m in items)
        {
            // Organisation upsert (Name = eindeutiger Schlüssel)
            Organization? org = null;
            if (!string.IsNullOrWhiteSpace(m.OrganizationName))
            {
                org = await _ctx.Organizations.FirstOrDefaultAsync(o => o.Name.ToLower() == m.OrganizationName.ToLower(), ct);
                if (org == null)
                {
                    org = new Organization
                    {
                        Id = Guid.NewGuid(),
                        Name = m.OrganizationName!,
                        AddressStreet = m.EventAddressStreet,
                        AddressCity = m.EventAddressCity,
                        AddressZip = m.EventAddressZip,
                        RegionId = TryParseInt(m.EventAddressState)
                    };
                    _ctx.Organizations.Add(org);
                }
                else
                {
                    // Optionales Update der Adresse falls leer
                    org.AddressStreet ??= m.EventAddressStreet;
                    org.AddressCity ??= m.EventAddressCity;
                    org.AddressZip ??= m.EventAddressZip;
                    org.RegionId ??= TryParseInt(m.EventAddressState);
                }
            }

            // Kontakt upsert (Email bevorzugt, sonst Phone)
            Contact? contact = null;
            if (!string.IsNullOrWhiteSpace(m.EventContactEmail) || !string.IsNullOrWhiteSpace(m.EventContactPhone))
            {
                if (!string.IsNullOrWhiteSpace(m.EventContactEmail))
                {
                    contact = await _ctx.Contacts.FirstOrDefaultAsync(c => c.Email == m.EventContactEmail, ct);
                }
                if (contact == null && !string.IsNullOrWhiteSpace(m.EventContactPhone))
                {
                    contact = await _ctx.Contacts.FirstOrDefaultAsync(c => c.Phone == m.EventContactPhone, ct);
                }
                if (contact == null)
                {
                    contact = new Contact
                    {
                        Id = Guid.NewGuid(),
                        Name = m.EventContactEmail ?? m.EventContactPhone ?? "Imported Contact",
                        Email = m.EventContactEmail,
                        Phone = m.EventContactPhone,
                        Organization = org
                    };
                    _ctx.Contacts.Add(contact);
                }
            }

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = string.IsNullOrWhiteSpace(m.EventTitle) ? "Imported Event" : m.EventTitle.Trim(),
                Description = m.EventDescription,
                DateStart = m.EventStart,
                DateEnd = m.EventEnd,
                EventLink = m.EventLink,
                Topics = m.EventTopics,
                TargetAudience = m.EventTargetAudience,
                IsOnline = m.EventIsOnline,
                Organization = org,
                Contact = contact,
                Status = EventStatus.Approved,
                ModifiedAt = DateTime.UtcNow,
                CreatedBy = "import",
                ModifiedBy = "import"
            };
            _ctx.Events.Add(ev);
            created++;
        }
        await _ctx.SaveChangesAsync(ct);
        return created;
    }

    private static int? TryParseInt(string? s) => int.TryParse(s, out var v) ? v : null;
}
