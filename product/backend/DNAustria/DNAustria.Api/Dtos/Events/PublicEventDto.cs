using System.Text.Json.Serialization;

namespace DNAustria.Api.Dtos.Events;

public record PublicEventDto
{
    [JsonPropertyName("event_title")]
    public required string EventTitle { get; init; }

    [JsonPropertyName("event_description")]
    public required string EventDescription { get; init; }

    [JsonPropertyName("event_link")]
    public required string EventLink { get; init; }

    [JsonPropertyName("event_target_audience")]
    public required IReadOnlyList<int> EventTargetAudience { get; init; }

    [JsonPropertyName("event_topics")]
    public required IReadOnlyList<int> EventTopics { get; init; }

    [JsonPropertyName("event_start")]
    public required string EventStart { get; init; }

    [JsonPropertyName("event_end")]
    public required string EventEnd { get; init; }

    [JsonPropertyName("event_classification")]
    public required string EventClassification { get; init; }

    [JsonPropertyName("event_has_fees")]
    public required bool EventHasFees { get; init; }

    [JsonPropertyName("event_is_online")]
    public required bool EventIsOnline { get; init; }

    [JsonPropertyName("organization_name")]
    public required string OrganizationName { get; init; }

    [JsonPropertyName("program_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProgramName { get; init; }

    [JsonPropertyName("event_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventFormat { get; init; }

    [JsonPropertyName("event_school_bookable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? EventSchoolBookable { get; init; }

    [JsonPropertyName("event_age_minimum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EventAgeMinimum { get; init; }

    [JsonPropertyName("event_age_maximum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EventAgeMaximum { get; init; }

    [JsonPropertyName("event_location_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventLocationName { get; init; }

    [JsonPropertyName("event_address_street")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventAddressStreet { get; init; }

    [JsonPropertyName("event_address_city")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventAddressCity { get; init; }

    [JsonPropertyName("event_address_zip")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventAddressZip { get; init; }

    [JsonPropertyName("event_address_state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventAddressState { get; init; }

    [JsonPropertyName("event_contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventContactName { get; init; }

    [JsonPropertyName("event_contact_org")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventContactOrg { get; init; }

    [JsonPropertyName("event_contact_email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventContactEmail { get; init; }

    [JsonPropertyName("event_contact_phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EventContactPhone { get; init; }

    [JsonPropertyName("location")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<double>? Location { get; init; }
}
