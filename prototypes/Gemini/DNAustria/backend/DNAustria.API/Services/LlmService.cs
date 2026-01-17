using DNAustria.API.Models;
using System.Text.Json;
using System.Text;

namespace DNAustria.API.Services
{
    public class LlmService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LlmService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<Event> ExtractEventFromTextAsync(string text)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
            {
                // Fallback to mock if no key provided
                return await ExtractEventFromTextMockAsync(text);
            }

            var prompt = $@"
Extract the following event details from the text below and return them as a JSON object.
Fields: Title, Description, DateStart (ISO 8601), DateEnd (ISO 8601), EventLink, IsOnline (boolean).
For Classification, assume 'Default' (value 0).
For Status, assume 'Draft' (value 0).
If a field is missing, make a best guess or leave it empty/null.
Use the current year 2026 if the year is missing.

Text:
{text}

JSON Output:
";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key={apiKey}", content);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                
                // Parse the Gemini response structure
                using var doc = JsonDocument.Parse(responseString);
                
                // Navigate to: candidates[0].content.parts[0].text
                var candidates = doc.RootElement.GetProperty("candidates");
                if (candidates.GetArrayLength() == 0) return await ExtractEventFromTextMockAsync(text);
                
                var contentElement = candidates[0].GetProperty("content");
                var parts = contentElement.GetProperty("parts");
                if (parts.GetArrayLength() == 0) return await ExtractEventFromTextMockAsync(text);
                
                var textPart = parts[0].GetProperty("text").GetString();
                if (string.IsNullOrEmpty(textPart)) return await ExtractEventFromTextMockAsync(text);

                // Clean up Markdown JSON blocks if present
                textPart = textPart.Replace("```json", "").Replace("```", "").Trim();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var extractedData = JsonSerializer.Deserialize<EventDto>(textPart, options);

                if (extractedData == null) return await ExtractEventFromTextMockAsync(text);

                return new Event
                {
                    Title = extractedData.Title ?? "Untitled Event",
                    Description = extractedData.Description ?? text,
                    DateStart = extractedData.DateStart,
                    DateEnd = extractedData.DateEnd,
                    EventLink = extractedData.EventLink ?? "",
                    IsOnline = extractedData.IsOnline,
                    Classification = EventClassification.Scheduled, // Enum mapping logic if needed
                    Status = EventStatus.Draft,
                    ProgramName = extractedData.ProgramName,
                    Format = extractedData.Format,
                    AgeMinimum = extractedData.AgeMinimum,
                    AgeMaximum = extractedData.AgeMaximum
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LLM Error: {ex.Message}");
                // Fallback on error
                return await ExtractEventFromTextMockAsync(text);
            }
        }

        private async Task<Event> ExtractEventFromTextMockAsync(string text)
        {
            // Simulate LLM processing delay
            await Task.Delay(1000);

            // Simple heuristic extraction (Mocking LLM)
            var extractedEvent = new Event
            {
                Title = "Extracted Event from Text (Mock)",
                Description = text,
                DateStart = DateTime.UtcNow.AddDays(7),
                DateEnd = DateTime.UtcNow.AddDays(7).AddHours(2),
                Status = EventStatus.Draft,
                Classification = EventClassification.Scheduled,
                Topics = new List<EventTopic> { EventTopic.DigitalisierungKuenstlicheIntelligenzITTechnik },
                TargetAudience = new List<TargetAudience> { TargetAudience.Erwachsene }
            };

            if (text.Contains("Conference", StringComparison.OrdinalIgnoreCase))
            {
                extractedEvent.Title = "Extracted Conference";
                extractedEvent.Topics = new List<EventTopic> { EventTopic.NaturwissenschaftKlimaUmwelt };
            }
            else if (text.Contains("Workshop", StringComparison.OrdinalIgnoreCase))
            {
                extractedEvent.Title = "Extracted Workshop";
                extractedEvent.Topics = new List<EventTopic> { EventTopic.DigitalisierungKuenstlicheIntelligenzITTechnik };
            }

            return extractedEvent;
        }

        // DTO to handle loose binding from LLM JSON
        private class EventDto
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public string? EventLink { get; set; }
            public bool IsOnline { get; set; }
            public string? ProgramName { get; set; }
            public string? Format { get; set; }
            public int? AgeMinimum { get; set; }
            public int? AgeMaximum { get; set; }
        }
    }
}
