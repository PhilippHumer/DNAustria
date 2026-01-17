using System.Text.RegularExpressions;
using DNAustria.Backend.Dtos;

namespace DNAustria.Backend.Services;

// NOTE: Stub implementation. Extend to call a real LLM (OpenAI, Azure, etc.) using secrets/env vars.
public class StubLLMService : ILLMService
{
    public Task<EventCreateDto> ParseEventAsync(string input, bool isHtml)
    {
        // Very simple heuristics: find title as first heading, dates as first date-like patterns, url as first http
        var dto = new EventCreateDto();

        // Try to strip HTML if needed
        var text = isHtml ? StripHtml(input) : input;

        // Title: first non-empty line
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        if (lines.Count > 0) dto.Title = lines[0];

        // Url
        var m = Regex.Match(text, "https?:\\/\\/[\\S]+", RegexOptions.IgnoreCase);
        if (m.Success) dto.EventLink = m.Value;

        // Dates: naive search for date ranges like 2026-01-01 or dd.MM.yyyy or Month names
        var dateMatches = Regex.Matches(text, "\\d{4}-\\d{2}-\\d{2}|");

        // Fallback: find two dates with day and month (e.g., 12.01.2026)
        var dm = Regex.Matches(text, "\\d{1,2}[.\\/\\-]\\d{1,2}[.\\/\\-]\\d{2,4}");
        if (dm.Count >= 1)
        {
            if (DateTime.TryParse(dm[0].Value, out var d0)) dto.DateStart = d0;
            if (dm.Count > 1 && DateTime.TryParse(dm[1].Value, out var d1)) dto.DateEnd = d1;
            else dto.DateEnd = dto.DateStart.AddHours(2);
        }
        else
        {
            dto.DateStart = DateTime.UtcNow;
            dto.DateEnd = dto.DateStart.AddHours(2);
        }

        // Description: next few lines (safe join to avoid exceptions if few lines)
        dto.Description = string.Join("\n", lines.Skip(1).Take(5));

        return Task.FromResult(dto);
    }

    private string StripHtml(string html)
    {
        return Regex.Replace(html, "<script.*?>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase)
                    .Replace("<", " <").Replace("\n", " ")
                    .Replace("\r", " ");
    }
}