using System.Text.Json.Serialization;

namespace DNAustria.Backend.Dtos;

public class ExportEventsResultDto
{
    public List<ExportEventDto> Events { get; set; } = new();
}

public class ExportEventDto
{
    [JsonPropertyName("event_title")]
    public string Event_Title { get; set; } = string.Empty;

    [JsonPropertyName("event_description")]
    public string Event_Description { get; set; } = string.Empty;

    [JsonPropertyName("event_link")]
    public string? Event_Link { get; set; }

    [JsonPropertyName("event_target_audience")]
    public List<int> Event_Target_Audience { get; set; } = new();

    [JsonPropertyName("event_topics")]
    public List<int> Event_Topics { get; set; } = new();

    [JsonPropertyName("event_start")]
    public string Event_Start { get; set; } = string.Empty; // ISO 8601

    [JsonPropertyName("event_end")]
    public string Event_End { get; set; } = string.Empty;   // ISO 8601

    [JsonPropertyName("event_classification")]
    public string Event_Classification { get; set; } = string.Empty; // "scheduled" | "on-demand"

    [JsonPropertyName("event_has_fees")]
    public bool Event_Has_Fees { get; set; }

    [JsonPropertyName("event_is_online")]
    public bool Event_Is_Online { get; set; }

    [JsonPropertyName("organization_name")]
    public string Organization_Name { get; set; } = string.Empty;

    [JsonPropertyName("program_name")]
    public string? Program_Name { get; set; }

    [JsonPropertyName("event_format")]
    public string? Event_Format { get; set; }

    [JsonPropertyName("event_school_bookable")]
    public bool? Event_School_Bookable { get; set; }

    [JsonPropertyName("event_age_minimum")]
    public int? Event_Age_Minimum { get; set; }

    [JsonPropertyName("event_age_maximum")]
    public int? Event_Age_Maximum { get; set; }

    [JsonPropertyName("event_location_name")]
    public string? Event_Location_Name { get; set; }

    [JsonPropertyName("event_address_street")]
    public string? Event_Address_Street { get; set; }

    [JsonPropertyName("event_address_city")]
    public string? Event_Address_City { get; set; }

    [JsonPropertyName("event_address_zip")]
    public string? Event_Address_Zip { get; set; }

    [JsonPropertyName("event_address_state")]
    public string? Event_Address_State { get; set; }

    [JsonPropertyName("event_mint_region")]
    public int? Event_Mint_Region { get; set; }

    [JsonPropertyName("event_contact_name")]
    public string? Event_Contact_Name { get; set; }

    [JsonPropertyName("event_contact_org")]
    public string? Event_Contact_Org { get; set; }

    [JsonPropertyName("event_contact_email")]
    public string? Event_Contact_Email { get; set; }

    [JsonPropertyName("event_contact_phone")]
    public string? Event_Contact_Phone { get; set; }

    [JsonPropertyName("location")]
    public List<double>? Location { get; set; }

    [JsonPropertyName("group_id")]
    public string? Group_Id { get; set; }
}