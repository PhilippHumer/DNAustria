using System.Text.Json.Serialization;
using DNAustria.API.Models;

namespace DNAustria.API.Models.Dtos;

public class EventListResponse
{
    [JsonPropertyName("events")]
    public List<EventExportDto> Events { get; set; } = new();
}

public class EventExportDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("event_title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("event_description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("event_link")]
    public string EventLink { get; set; } = string.Empty;

    [JsonPropertyName("event_target_audience")]
    public List<int> TargetAudience { get; set; } = new();

    [JsonPropertyName("event_topics")]
    public List<int> Topics { get; set; } = new();

    [JsonPropertyName("event_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("event_end")]
    public DateTime DateEnd { get; set; }

    [JsonPropertyName("event_classification")]
    public string Classification { get; set; } = string.Empty;

    [JsonPropertyName("event_has_fees")]
    public bool Fees { get; set; }

    [JsonPropertyName("event_is_online")]
    public bool IsOnline { get; set; }

    [JsonPropertyName("organization_name")]
    public string? OrganizationName { get; set; }

    [JsonPropertyName("program_name")]
    public string ProgramName { get; set; } = string.Empty;

    [JsonPropertyName("event_format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("event_school_bookable")]
    public bool SchoolBookable { get; set; }

    [JsonPropertyName("event_age_minimum")]
    public int AgeMinimum { get; set; }

    [JsonPropertyName("event_age_maximum")]
    public int AgeMaximum { get; set; }

    [JsonPropertyName("event_location_name")]
    public string? LocationName { get; set; }

    [JsonPropertyName("event_address_street")]
    public string? Street { get; set; }

    [JsonPropertyName("event_address_city")]
    public string? City { get; set; }

    [JsonPropertyName("event_address_zip")]
    public string? Zip { get; set; }

    [JsonPropertyName("event_address_state")]
    public string? State { get; set; }

    [JsonPropertyName("event_mint_region")]
    public int? MintRegion { get; set; }

    [JsonPropertyName("event_contact_name")]
    public string? ContactName { get; set; }

    [JsonPropertyName("event_contact_org")]
    public string? ContactOrg { get; set; }

    [JsonPropertyName("event_contact_email")]
    public string? ContactEmail { get; set; }

    [JsonPropertyName("event_contact_phone")]
    public string? ContactPhone { get; set; }

    [JsonPropertyName("location")]
    public double[] LocationCoordinates { get; set; } = Array.Empty<double>();
}
