using DNAustria.Domain;
using DNAustria.Dal;

namespace DNAustria.Logic.Events;

public interface IEventLogic
{
    Task<PagedResult<Event>> GetAllAsync(string? name, EventStatus? status, int page, int pageSize);
    Task<Event?> GetByIdAsync(int id);
    Task<IReadOnlyList<EventHistoryEntry>> GetHistoryByEventIdAsync(int id);

    Task<Event> CreateAsync(
        Event entity,
        IEnumerable<int>? targetAudiences,
        IEnumerable<int>? topics);

    Task<Event?> UpdateAsync(
        int id,
        Event entity,
        IEnumerable<int>? targetAudiences,
        IEnumerable<int>? topics);

    Task<bool> DeleteAsync(int id);

    Task<Event?> UpdateStatusAsync(int id, EventStatus status);
    

    Task<IReadOnlyList<Dal.Models.Event>> HandlePublishEventsAsync();
}
