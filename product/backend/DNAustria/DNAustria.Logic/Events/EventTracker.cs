using DNAustria.Dal.Data;
using DNAustria.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Logic.Events;

public class EventTracker(AppDbContext db) : IEventTracker
{
    public async Task TrackAsync(int eventId, int userId, string action)
    {
        // Ensure the provided userId exists to avoid FK violations in event_history.
        int? resolvedUserId = null;
        try
        {
            resolvedUserId = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync();

            if (resolvedUserId is null)
            {
                // Fallback: use the first available user in the system
                resolvedUserId = await db.Users
                    .AsNoTracking()
                    .OrderBy(u => u.Id)
                    .Select(u => (int?)u.Id)
                    .FirstOrDefaultAsync();
            }
        }
        catch
        {
            resolvedUserId = null;
        }

        if (resolvedUserId is null)
        {
            // No user available — skip tracking to avoid DB constraint errors.
            return;
        }

        var historyEntry = new EventHistory
        {
            EventId = eventId,
            UserId = resolvedUserId.Value,
            Action = action,
            CreatedAt = DateTime.UtcNow
        };

        db.EventHistories.Add(historyEntry);
        await db.SaveChangesAsync();
    }
}
