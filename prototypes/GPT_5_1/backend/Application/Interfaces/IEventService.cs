using Application.DTOs;

namespace Application.Interfaces;

public interface IEventService
{
    Task<IEnumerable<EventDto>> ListAsync(string? search,string? filter,int page,int pageSize);
    Task<EventDto> CreateAsync(EventDto dto);
    Task<EventDto?> UpdateAsync(Guid id, EventDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<EventDto?> ImportAsync(string raw);
    Task<EventDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ExportEventDto>> PublicExportAsync();
}
