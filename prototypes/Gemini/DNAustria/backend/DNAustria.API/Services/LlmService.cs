using DNAustria.API.Models;
using System;
using System.Threading.Tasks;

namespace DNAustria.API.Services
{
    public class LlmService
    {
        public async Task<Event> ExtractEventFromTextAsync(string text)
        {
            // Simulate LLM processing delay
            await Task.Delay(1000);

            // Simple heuristic extraction (Mocking LLM)
            var extractedEvent = new Event
            {
                Title = "Extracted Event from Text",
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
    }
}
