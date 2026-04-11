namespace DNAustria.Domain;

public record EventHistoryEntry
{
    public required string Action { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Username { get; init; }
}
