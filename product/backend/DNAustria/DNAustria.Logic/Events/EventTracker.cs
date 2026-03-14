using DNAustria.Dal.Data;
using DNAustria.Dal.Models;

namespace DNAustria.Logic.Events;

public class EventTracker(AppDbContext db) : IEventTracker
{
    public async Task TrackAsync(int eventId, int userId, string action)
    {
        var historyEntry = new EventHistory
        {
            EventId = eventId,
            UserId = userId,
            Action = action,
            CreatedAt = DateTime.UtcNow
        };

        db.EventHistories.Add(historyEntry);
        await db.SaveChangesAsync();
    }
}