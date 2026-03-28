using DNAustria.Domain;
using DNAustria.Dal;

namespace DNAustria.Logic.Events;

public interface IEventLogic
{
    Task<IReadOnlyList<Event>> GetAllAsync(string? name);
    Task<Event?> GetByIdAsync(int id);

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