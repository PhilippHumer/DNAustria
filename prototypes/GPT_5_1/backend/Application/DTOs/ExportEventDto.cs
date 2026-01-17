using System.Text.Json.Serialization;

namespace Application.DTOs;

public class ExportEventDto
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
    [JsonPropertyName("location")] public float[]? Location { get; set; }
}
