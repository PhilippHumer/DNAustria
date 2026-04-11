namespace DNAustria.Api.Dtos.Events;

public record EventHistoryDto
{
    public required string Action { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Username { get; init; }
}
