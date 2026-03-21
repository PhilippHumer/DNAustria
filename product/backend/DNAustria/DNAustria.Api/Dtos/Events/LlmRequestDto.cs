namespace DNAustria.Api.Dtos;

/// <summary>
/// DTO for LLM requests. Use <see cref="ExamplePrompt"/> as a template when asking the LLM
/// to produce an event as JSON matching the existing InsertEventDto structure.
/// </summary>
public class LlmRequestDto
{
    public string? Prompt { get; set; }

    /// <summary>
    /// Example prompt template that instructs the model to return only a single JSON object
    /// matching the InsertEventDto fields. Adjust the values as needed.
    /// </summary>
    public static string ExamplePrompt =>
        "Respond ONLY with a single JSON object matching the InsertEventDto structure. " +
        "Do NOT include any explanatory text. Use ISO 8601 for dates. Example output:\n" +
        "{\n" +
        "  \"Name\": \"Event title\",\n" +
        "  \"Description\": \"Short description...\",\n" +
        "  \"Link\": \"https://example.com\",\n" +
        "  \"StartDate\": \"2026-04-01T10:00:00Z\",\n" +
        "  \"EndDate\": \"2026-04-01T12:00:00Z\",\n" +
        "  \"Classification\": 0,\n" +
        "  \"Status\": 0,\n" +
        "  \"HasFees\": false,\n" +
        "  \"IsOnline\": false,\n" +
        "  \"ProgramName\": null,\n" +
        "  \"Format\": null,\n" +
        "  \"SchoolBookable\": false,\n" +
        "  \"AgeMinimum\": null,\n" +
        "  \"AgeMaximum\": null,\n" +
        "  \"Organization\": null,\n" +
        "  \"Location\": null,\n" +
        "  \"Contact\": null,\n" +
        "  \"TargetAudiences\": [],\n" +
        "  \"Topics\": []\n" +
        "}";
}
