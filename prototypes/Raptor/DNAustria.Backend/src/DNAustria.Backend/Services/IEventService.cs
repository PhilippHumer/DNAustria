using DNAustria.Backend.Dtos;
using DNAustria.Backend.Models;

namespace DNAustria.Backend.Services;

public interface IEventService
{
    Task<IEnumerable<EventListDto>> GetEventsAsync(EventStatus? status = null, string? q = null);
    Task<EventDetailDto?> GetEventAsync(Guid id);
    Task<EventDetailDto> CreateEventAsync(EventCreateDto dto);
    Task<EventDetailDto?> UpdateEventAsync(Guid id, EventCreateDto dto);
    Task<bool> DeleteEventAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, EventStatus status);
    Task<EventCreateDto> ParseEventAsync(string input, bool isHtml);

    Task<Dtos.ExportEventsResultDto> GetApprovedEventsExportAsync();
}