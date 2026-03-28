using System.Globalization;
using System.Text;
using System.Text.Json;

namespace DNAustria.Logic;

public class EventExtractionService(ILLMLogic llmLogic) : IEventExtractionService
{
    private readonly ILLMLogic _llmLogic = llmLogic ?? throw new ArgumentNullException(nameof(llmLogic));

    public async Task<EventExtractionResult> ExtractEventAsync(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            return new EventExtractionResult(false, null, "Text is required.");

        var prompt = BuildTransformPrompt(inputText);
        var llmText = await _llmLogic.GetChatCompletionAsync(prompt);
        if (string.IsNullOrWhiteSpace(llmText))
            return new EventExtractionResult(false, null, "LLM returned empty response.");

        var start = llmText.IndexOf('{');
        var end = llmText.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            var completed = await TryCompleteJsonAsync(llmText);
            if (string.IsNullOrWhiteSpace(completed))
                return new EventExtractionResult(false, null, "LLM response appears truncated or invalid JSON. Increase OpenAI MaxTokens and retry. Partial response: " + llmText);

            llmText = completed;
            start = llmText.IndexOf('{');
            end = llmText.LastIndexOf('}');
            if (start < 0 || end <= start)
                return new EventExtractionResult(false, null, "LLM response appears truncated or invalid JSON even after retry. Partial response: " + llmText);
        }

        var json = llmText[start..(end + 1)];

        ExtractedEventData data;
        try
        {
            data = ParseDataFromJson(json);
        }
        catch (JsonException)
        {
            var repaired = TryRepairJson(json);
            try
            {
                data = ParseDataFromJson(repaired);
            }
            catch (JsonException)
            {
                return new EventExtractionResult(false, null, "Failed to parse JSON from LLM response.");
            }
        }

        if (string.IsNullOrWhiteSpace(data.Name))
            return new EventExtractionResult(false, null, "LLM response is missing required field: Name.");

        data = data with
        {
            Description = string.IsNullOrWhiteSpace(data.Description) ? inputText : data.Description,
            Link = string.IsNullOrWhiteSpace(data.Link) ? "https://example.com" : data.Link,
            ProgramName = string.IsNullOrWhiteSpace(data.ProgramName) ? "General" : data.ProgramName,
            Format = string.IsNullOrWhiteSpace(data.Format) ? "Standard" : data.Format,
            StartDate = EnsureUtc(data.StartDate),
            EndDate = EnsureUtc(data.EndDate)
        };

        return new EventExtractionResult(true, data, null);
    }

    private static string BuildTransformPrompt(string inputText)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are an assistant that extracts structured event data from unstructured text.");
        sb.AppendLine("Task: Read the provided text and return ONLY a single JSON object matching this schema and nothing else.");
        sb.AppendLine("Rules:");
        sb.AppendLine("- Return minified JSON (single line).");
        sb.AppendLine("- Use ISO 8601 UTC dates.");
        sb.AppendLine("- ALWAYS provide non-empty values for Name, Description, Link, ProgramName and Format.");
        sb.AppendLine("- Use null for missing optional IDs.");
        sb.AppendLine("Schema fields:");
        sb.AppendLine("Name, Description, Link, StartDate, EndDate, Classification, Status, HasFees, IsOnline, Organization, ProgramName, Format, SchoolBookable, AgeMinimum, AgeMaximum, Location, Contact, TargetAudiences, Topics");
        sb.AppendLine("Input text:");
        sb.AppendLine(inputText);
        sb.AppendLine("Output JSON:");
        return sb.ToString();
    }

    private ExtractedEventData ParseDataFromJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Root is not object");

        return new ExtractedEventData(
            Name: GetString(root, "Name") ?? string.Empty,
            Description: GetString(root, "Description") ?? string.Empty,
            Link: GetString(root, "Link") ?? string.Empty,
            StartDate: GetDateTime(root, "StartDate") ?? DateTime.UtcNow,
            EndDate: GetDateTime(root, "EndDate") ?? DateTime.UtcNow.AddHours(2),
            Classification: GetInt(root, "Classification") ?? 0,
            Status: GetInt(root, "Status") ?? 0,
            HasFees: GetBool(root, "HasFees") ?? false,
            IsOnline: GetBool(root, "IsOnline") ?? false,
            Organization: GetNullableInt(root, "Organization") ?? GetNullableInt(root, "OrganizationId"),
            ProgramName: GetString(root, "ProgramName") ?? string.Empty,
            Format: GetString(root, "Format") ?? string.Empty,
            SchoolBookable: GetBool(root, "SchoolBookable") ?? false,
            AgeMinimum: GetInt(root, "AgeMinimum") ?? 0,
            AgeMaximum: GetInt(root, "AgeMaximum") ?? 0,
            Location: GetNullableInt(root, "Location") ?? GetNullableInt(root, "LocationId"),
            Contact: GetNullableInt(root, "Contact") ?? GetNullableInt(root, "ContactId"),
            TargetAudiences: GetIntList(root, "TargetAudiences"),
            Topics: GetIntList(root, "Topics"));
    }

    private static string? GetString(JsonElement root, string name)
    {
        if (!TryGetPropertyCaseInsensitive(root, name, out var value)) return null;
        return value.ValueKind == JsonValueKind.Null ? null : value.GetString();
    }

    private static int? GetInt(JsonElement root, string name)
    {
        if (!TryGetPropertyCaseInsensitive(root, name, out var value) || value.ValueKind == JsonValueKind.Null)
            return null;

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var n)) return n;
        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out n)) return n;
        return null;
    }

    private static int? GetNullableInt(JsonElement root, string name) => GetInt(root, name);

    private static bool? GetBool(JsonElement root, string name)
    {
        if (!TryGetPropertyCaseInsensitive(root, name, out var value) || value.ValueKind == JsonValueKind.Null)
            return null;

        if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
            return value.GetBoolean();

        if (value.ValueKind == JsonValueKind.String && bool.TryParse(value.GetString(), out var b))
            return b;

        return null;
    }

    private static DateTime? GetDateTime(JsonElement root, string name)
    {
        if (!TryGetPropertyCaseInsensitive(root, name, out var value) || value.ValueKind == JsonValueKind.Null)
            return null;

        if (value.ValueKind == JsonValueKind.String)
        {
            var text = value.GetString();
            if (string.IsNullOrWhiteSpace(text)) return null;

            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
                return parsed.Kind == DateTimeKind.Utc ? parsed : parsed.ToUniversalTime();
        }

        return null;
    }

    private static List<int> GetIntList(JsonElement root, string name)
    {
        var result = new List<int>();
        if (!TryGetPropertyCaseInsensitive(root, name, out var value) || value.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in value.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var n))
                result.Add(n);
            else if (item.ValueKind == JsonValueKind.String && int.TryParse(item.GetString(), out n))
                result.Add(n);
        }

        return result;
    }

    private static bool TryGetPropertyCaseInsensitive(JsonElement root, string name, out JsonElement value)
    {
        foreach (var property in root.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value
        };
    }

    private static string TryRepairJson(string partial)
    {
        if (string.IsNullOrWhiteSpace(partial)) return partial;

        partial = partial.TrimEnd();
        if (partial.EndsWith(",")) partial = partial[..^1];

        var openBraces = partial.Count(c => c == '{');
        var closeBraces = partial.Count(c => c == '}');
        var openBrackets = partial.Count(c => c == '[');
        var closeBrackets = partial.Count(c => c == ']');

        var sb = new StringBuilder(partial);
        for (int i = 0; i < openBrackets - closeBrackets; i++) sb.Append(']');
        for (int i = 0; i < openBraces - closeBraces; i++) sb.Append('}');

        return sb.ToString();
    }

    private async Task<string?> TryCompleteJsonAsync(string partial)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("The previous response was truncated. Complete the JSON object only, do not add any explanation.");
            sb.AppendLine("Partial JSON:");
            sb.AppendLine(partial);
            return await _llmLogic.GetChatCompletionAsync(sb.ToString());
        }
        catch
        {
            return null;
        }
    }
}
