namespace DNAustria.Api.Dtos.Events;

public record PagedEventsDto
{
    public required IReadOnlyList<EventDto> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
}
