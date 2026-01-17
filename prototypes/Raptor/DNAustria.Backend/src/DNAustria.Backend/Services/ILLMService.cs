using DNAustria.Backend.Dtos;

namespace DNAustria.Backend.Services;

public interface ILLMService
{
    Task<EventCreateDto> ParseEventAsync(string input, bool isHtml);
}