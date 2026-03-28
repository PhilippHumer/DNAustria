using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNAustria.Api.Dtos;

/// <summary>
/// DTO for LLM requests. Use <see cref="BuildTransformPrompt(string)"/> to build a prompt
/// that transforms unstructured text into a JSON matching the InsertEventDto structure.
/// </summary>
public class LlmRequestDto
{
    public string? Text { get; set; }
    public string? Prompt { get; set; }

    public string? GetInputText() => string.IsNullOrWhiteSpace(Text) ? Prompt : Text;

    /// <summary>
    /// Build a prompt that instructs the model to transform the provided free-form text into
    /// a single JSON object matching the InsertEventDto. The model must return ONLY the JSON.
    /// </summary>
    /// <param name="inputText">Unstructured text describing an event.</param>
    public static string BuildTransformPrompt(string inputText)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are an assistant that extracts structured event data from unstructured text.");
        sb.AppendLine("Task: Read the provided text and return ONLY a single JSON object that matches the InsertEventDto structure (no explanation, no code fences).");
        sb.AppendLine("Rules:");
        sb.AppendLine("- Return minified JSON (single line, no pretty formatting) to reduce token usage.");
        sb.AppendLine("- Use ISO 8601 format for dates (e.g. 2026-04-01T10:00:00Z).");
        sb.AppendLine("- Use null for missing optional values and empty arrays for lists.");
        sb.AppendLine("- Map booleans and integers appropriately.");
        sb.AppendLine("- ALWAYS provide non-empty values for Name, Description, Link, ProgramName and Format.");
        sb.AppendLine("- Extract organization/location/contact when explicit numeric IDs are present; otherwise use null.");
        sb.AppendLine();
        sb.AppendLine("Structure (InsertEventDto):");
        sb.AppendLine("{\n  \"Name\": string,\n  \"Description\": string,\n  \"Link\": string,\n  \"StartDate\": string (ISO 8601),\n  \"EndDate\": string (ISO 8601),\n  \"Classification\": int,\n  \"Status\": int,\n  \"HasFees\": bool,\n  \"IsOnline\": bool,\n  \"Organization\": int or null,\n  \"ProgramName\": string or null,\n  \"Format\": string or null,\n  \"SchoolBookable\": bool,\n  \"AgeMinimum\": int or null,\n  \"AgeMaximum\": int or null,\n  \"Location\": int or null,\n  \"Contact\": int or null,\n  \"TargetAudiences\": array of ints,\n  \"Topics\": array of ints\n}");
        sb.AppendLine();
        sb.AppendLine("Input text:");
        sb.AppendLine(inputText ?? string.Empty);
        sb.AppendLine();
        sb.AppendLine("Output JSON:");

        return sb.ToString();
    }

    /// <summary>
    /// Build a prompt using an example InsertEventDto to show the model expected field values.
    /// The provided example's values will be serialized and included in the prompt so the model
    /// can follow the exact shape and example values when converting the input text.
    /// </summary>
    public static string BuildTransformPrompt(InsertEventDto example, string inputText)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var exampleJson = JsonSerializer.Serialize(example, options);

        var sb = new StringBuilder();
        sb.AppendLine("You are an assistant that extracts structured event data from unstructured text.");
        sb.AppendLine("Task: Read the provided text and return ONLY a single JSON object that matches the InsertEventDto structure (no explanation, no code fences).");
        sb.AppendLine("Rules:");
        sb.AppendLine("- Return minified JSON (single line, no pretty formatting) to reduce token usage.");
        sb.AppendLine("- Use ISO 8601 format for dates (e.g. 2026-04-01T10:00:00Z).");
        sb.AppendLine("- Use null for missing optional values and empty arrays for lists.");
        sb.AppendLine("- Map booleans and integers appropriately.");
        sb.AppendLine("- ALWAYS provide non-empty values for Name, Description, Link, ProgramName and Format.");
        sb.AppendLine("- Extract organization/location/contact when explicit numeric IDs are present; otherwise use null.");
        sb.AppendLine();
        sb.AppendLine("Example to follow (InsertEventDto with sample values):");
        sb.AppendLine(exampleJson);
        sb.AppendLine();
        sb.AppendLine("Input text:");
        sb.AppendLine(inputText ?? string.Empty);
        sb.AppendLine();
        sb.AppendLine("Output JSON:");

        return sb.ToString();
    }
}
