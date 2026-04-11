namespace DNAustria.Api.Dtos.Events;

public record EventDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Link { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required int Classification { get; init; }
    public required int Status { get; init; }
    public required bool HasFees { get; init; }
    public required bool IsOnline { get; init; }
    public required int? Organization { get; init; }
    public string? ProgramName { get; init; }
    public string? Format { get; init; }
    public required bool SchoolBookable { get; init; }
    public required int AgeMinimum { get; init; }
    public required int AgeMaximum { get; init; }
    public required int? Location { get; init; }
    public required int? Contact { get; init; }
    public required IReadOnlyList<int> TargetAudiences { get; init; }
    public required IReadOnlyList<int> Topics { get; init; }
    public IReadOnlyList<EventHistoryDto> History { get; init; } = Array.Empty<EventHistoryDto>();
}
