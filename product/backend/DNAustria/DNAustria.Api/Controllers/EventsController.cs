using DNAustria.Api.Dtos;
using DNAustria.Api.Dtos.Events;
using DNAustria.Domain;
using DNAustria.Logic;
using DNAustria.Logic.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace DNAustria.Api.Controllers;

[ApiController]
public class EventsController(IEventLogic eventLogic, ILLMLogic llmLogic, IConfiguration configuration) : ControllerBase
{
    private readonly IEventLogic _eventLogic = eventLogic ?? throw new ArgumentNullException(nameof(eventLogic));
    private readonly ILLMLogic _llmLogic = llmLogic ?? throw new ArgumentNullException(nameof(llmLogic));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));


    [HttpGet("api/events")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll([FromQuery] string? name)
    {
        var events = await _eventLogic.GetAllAsync(name);
        var dtos = events.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpPost("api/events/llm")]
    public async Task<ActionResult<EventDto>> PostLlm([FromBody] LlmRequestDto? req)
    {
        var inputText = req?.GetInputText();
        if (string.IsNullOrWhiteSpace(inputText))
            return BadRequest("Text is required. Send JSON: { \"text\": \"...\" } (or legacy { \"prompt\": \"...\" }).");

        try
        {
            var example = new InsertEventDto
            {
                Name = "Event title",
                Description = "Short description",
                Link = "https://example.com",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(2),
                Classification = 0,
                Status = 0,
                HasFees = false,
                IsOnline = false,
                Organization = null,
                ProgramName = null,
                Format = null,
                SchoolBookable = false,
                AgeMinimum = 0,
                AgeMaximum = 99,
                Location = null,
                Contact = null,
                TargetAudiences = new List<int>(),
                Topics = new List<int>()
            };

            var prompt = LlmRequestDto.BuildTransformPrompt(example, inputText);
            var llmText = await _llmLogic.GetChatCompletionAsync(prompt);
            if (string.IsNullOrWhiteSpace(llmText))
                return BadRequest("LLM returned empty response.");

            // try to extract JSON object from LLM response
            var start = llmText.IndexOf('{');
            var end = llmText.LastIndexOf('}');
            if (start < 0 || end <= start)
            {
                // Attempt to ask the LLM to complete the truncated JSON
                var completed = await TryCompleteJsonAsync(llmText);
                if (string.IsNullOrWhiteSpace(completed))
                    return BadRequest("LLM response appears truncated or invalid JSON. Increase OpenAI MaxTokens and retry. Partial response: " + llmText);

                llmText = completed;
                start = llmText.IndexOf('{');
                end = llmText.LastIndexOf('}');
                if (start < 0 || end <= start)
                    return BadRequest("LLM response appears truncated or invalid JSON even after retry. Partial response: " + llmText);
            }

            var json = llmText[start..(end + 1)];

            InsertEventDto dto;
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                    return BadRequest("LLM JSON root must be an object.");

                dto = new InsertEventDto
                {
                    Name = GetString(root, "Name") ?? string.Empty,
                    Description = GetString(root, "Description") ?? string.Empty,
                    Link = GetString(root, "Link") ?? string.Empty,
                    StartDate = GetDateTime(root, "StartDate") ?? DateTime.UtcNow,
                    EndDate = GetDateTime(root, "EndDate") ?? DateTime.UtcNow.AddHours(2),
                    Classification = GetInt(root, "Classification") ?? 0,
                    Status = GetInt(root, "Status") ?? 0,
                    HasFees = GetBool(root, "HasFees") ?? false,
                    IsOnline = GetBool(root, "IsOnline") ?? false,
                    Organization = GetNullableInt(root, "Organization"),
                    ProgramName = GetString(root, "ProgramName") ?? string.Empty,
                    Format = GetString(root, "Format") ?? string.Empty,
                    SchoolBookable = GetBool(root, "SchoolBookable") ?? false,
                    AgeMinimum = GetInt(root, "AgeMinimum") ?? 0,
                    AgeMaximum = GetInt(root, "AgeMaximum") ?? 0,
                    Location = GetNullableInt(root, "Location"),
                    Contact = GetNullableInt(root, "Contact"),
                    TargetAudiences = GetIntList(root, "TargetAudiences"),
                    Topics = GetIntList(root, "Topics")
                };
            }
            catch (JsonException)
            {
                // Attempt to repair common truncation issues (e.g. trailing partial property)
                var repaired = TryRepairJson(json);
                try
                {
                    using var doc = JsonDocument.Parse(repaired);
                    var root = doc.RootElement;
                    if (root.ValueKind != JsonValueKind.Object)
                        return BadRequest("LLM JSON root must be an object.");

                    dto = new InsertEventDto
                    {
                        Name = GetString(root, "Name") ?? string.Empty,
                        Description = GetString(root, "Description") ?? string.Empty,
                        Link = GetString(root, "Link") ?? string.Empty,
                        StartDate = GetDateTime(root, "StartDate") ?? DateTime.UtcNow,
                        EndDate = GetDateTime(root, "EndDate") ?? DateTime.UtcNow.AddHours(2),
                        Classification = GetInt(root, "Classification") ?? 0,
                        Status = GetInt(root, "Status") ?? 0,
                        HasFees = GetBool(root, "HasFees") ?? false,
                        IsOnline = GetBool(root, "IsOnline") ?? false,
                        Organization = GetNullableInt(root, "Organization"),
                        ProgramName = GetString(root, "ProgramName") ?? string.Empty,
                        Format = GetString(root, "Format") ?? string.Empty,
                        SchoolBookable = GetBool(root, "SchoolBookable") ?? false,
                        AgeMinimum = GetInt(root, "AgeMinimum") ?? 0,
                        AgeMaximum = GetInt(root, "AgeMaximum") ?? 0,
                        Location = GetNullableInt(root, "Location"),
                        Contact = GetNullableInt(root, "Contact"),
                        TargetAudiences = GetIntList(root, "TargetAudiences"),
                        Topics = GetIntList(root, "Topics")
                    };
                }
                catch (JsonException)
                {
                    return BadRequest("Failed to parse JSON from LLM response.");
                }
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("LLM response is missing required field: Name.");

            // Fill required domain values when LLM omits them.
            // Domain.Event requires non-empty Description, Link, ProgramName and Format.
            dto = dto with
            {
                Description = string.IsNullOrWhiteSpace(dto.Description) ? inputText : dto.Description,
                Link = string.IsNullOrWhiteSpace(dto.Link) ? "https://example.com" : dto.Link,
                ProgramName = string.IsNullOrWhiteSpace(dto.ProgramName) ? "General" : dto.ProgramName,
                Format = string.IsNullOrWhiteSpace(dto.Format) ? "Standard" : dto.Format
            };

            // Map DTO to domain (same as Create endpoint)
            var startDateUtc = EnsureUtc(dto.StartDate);
            var endDateUtc = EnsureUtc(dto.EndDate);

            var domain = new Event(
                name: dto.Name,
                description: dto.Description,
                link: dto.Link,
                startDate: startDateUtc,
                endDate: endDateUtc,
                classification: (EventClassification)dto.Classification,
                status: (EventStatus)dto.Status,
                hasFees: dto.HasFees,
                isOnline: dto.IsOnline,
                programName: dto.ProgramName,
                format: dto.Format,
                schoolBookable: dto.SchoolBookable,
                ageMinimum: dto.AgeMinimum,
                ageMaximum: dto.AgeMaximum,
                organizationId: dto.Organization,
                locationId: dto.Location,
                contactId: dto.Contact
            );

            var created = await _eventLogic.CreateAsync(
                domain,
                targetAudiences: dto.TargetAudiences,
                topics: dto.Topics);

            var resultDto = MapToDto(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, resultDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Failed to save event. Check referenced IDs (Organization/Location/Contact) and enum values. Details: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

    private static string? GetString(JsonElement root, string name)
    {
        if (!TryGetPropertyCaseInsensitive(root, name, out var value))
            return null;

        return value.ValueKind == JsonValueKind.Null ? null : value.GetString();
    }

    private static int? GetInt(JsonElement root, string name)
    {
        if (!TryGetPropertyCaseInsensitive(root, name, out var value) || value.ValueKind == JsonValueKind.Null)
            return null;

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var n))
            return n;

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out n))
            return n;

        return null;
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
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var parsed))
            {
                return parsed.Kind == DateTimeKind.Utc
                    ? parsed
                    : parsed.ToUniversalTime();
            }
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

    // Try to fix truncated JSON by closing open arrays/objects and removing trailing incomplete token
    private static string TryRepairJson(string partial)
    {
        if (string.IsNullOrWhiteSpace(partial))
            return partial;

        // Trim whitespace
        partial = partial.TrimEnd();

        // If ends with a comma, remove it
        if (partial.EndsWith(","))
            partial = partial.Substring(0, partial.Length - 1);

        // If missing a closing bracket for arrays or objects, try to add them
        var openBraces = partial.Count(c => c == '{');
        var closeBraces = partial.Count(c => c == '}');
        var openBrackets = partial.Count(c => c == '[');
        var closeBrackets = partial.Count(c => c == ']');

        var sb = new StringBuilder(partial);
        // Close arrays first
        for (int i = 0; i < openBrackets - closeBrackets; i++) sb.Append(']');
        // Then close objects
        for (int i = 0; i < openBraces - closeBraces; i++) sb.Append('}');

        return sb.ToString();
    }

    private async Task<string?> TryCompleteJsonAsync(string partial)
    {
        try
        {
            // Build a short prompt asking the LLM to finish the previous JSON.
            var sb = new StringBuilder();
            sb.AppendLine("The previous response was truncated. Complete the JSON object only, do not add any explanation.");
            sb.AppendLine("Partial JSON:");
            sb.AppendLine(partial);

            var prompt = sb.ToString();

            // Use the same LLM logic implementation to get a completion
            var completion = await _llmLogic.GetChatCompletionAsync(prompt);
            return completion;
        }
        catch
        {
            return null;
        }
    }


    [HttpGet("api/events/{id:int}")]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var e = await _eventLogic.GetByIdAsync(id);
        if (e is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(e));
    }

    [HttpPost("api/events")]
    public async Task<ActionResult<EventDto>> Create([FromBody] InsertEventDto req)
    {
        var domain = new Event(
            name: req.Name,
            description: req.Description,
            link: req.Link,
            startDate: req.StartDate,
            endDate: req.EndDate,
            classification: (EventClassification)req.Classification,
            status: (EventStatus)req.Status,
            hasFees: req.HasFees,
            isOnline: req.IsOnline,
            programName: req.ProgramName,
            format: req.Format,
            schoolBookable: req.SchoolBookable,
            ageMinimum: req.AgeMinimum,
            ageMaximum: req.AgeMaximum,
            organizationId: req.Organization,
            locationId: req.Location,
            contactId: req.Contact
        );

        var created = await _eventLogic.CreateAsync(
            domain,
            targetAudiences: req.TargetAudiences,
            topics: req.Topics);

        var dto = MapToDto(created);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            dto);
    }

    [HttpPut("api/events/{id:int}")]
    public async Task<ActionResult<EventDto>> Update(int id, [FromBody] UpdateEventDto req)
    {
        if (!Enum.IsDefined(typeof(EventStatus), req.Status))
        {
            return BadRequest($"Invalid status value: {req.Status}");
        }
        
        var domain = new Event(
            name: req.Name,
            description: req.Description,
            link: req.Link,
            startDate: req.StartDate,
            endDate: req.EndDate,
            classification: (EventClassification)req.Classification,
            status: (EventStatus)req.Status,
            hasFees: req.HasFees,
            isOnline: req.IsOnline,
            programName: req.ProgramName,
            format: req.Format,
            schoolBookable: req.SchoolBookable,
            ageMinimum: req.AgeMinimum,
            ageMaximum: req.AgeMaximum,
            organizationId: req.Organization,
            locationId: req.Location,
            contactId: req.Contact
        );

        var updated = await _eventLogic.UpdateAsync(
            id,
            domain,
            targetAudiences: req.TargetAudiences,
            topics: req.Topics);

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(updated));
    }

    [HttpDelete("api/events/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _eventLogic.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("api/events/{id:int}/status")]
    public async Task<ActionResult<EventDto>> UpdateStatus(int id, [FromBody] UpdateEventStatusDto req)
    {
        if (!Enum.IsDefined(typeof(EventStatus), req.Status))
        {
            return BadRequest($"Invalid status value: {req.Status}");
        }
        
        var updated = await _eventLogic.UpdateStatusAsync(id, req.Status);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(updated));
    }

    private static EventDto MapToDto(Event e)
    {
        return new EventDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Link = e.Link,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Classification = (int)e.Classification,
            Status = (int)e.Status,
            HasFees = e.HasFees,
            IsOnline = e.IsOnline,
            Organization = e.OrganizationId,
            ProgramName = e.ProgramName,
            Format = e.Format,
            SchoolBookable = e.SchoolBookable,
            AgeMinimum = e.AgeMinimum,
            AgeMaximum = e.AgeMaximum,
            Location = e.LocationId,
            Contact = e.ContactId,
            TargetAudiences = e.TargetAudiences.Select(x => (int)x.TargetAudience).ToList(),
            Topics = e.Topics.Select(x => (int)x.Topic).ToList()
        };
    }
}