namespace DNAustria.Logic.Events;

public interface IEventTracker
{
    Task TrackAsync(int eventId, int userId, string action);
}