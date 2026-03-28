namespace DNAustria.Logic;

public interface IEventExtractionService
{
    Task<EventExtractionResult> ExtractEventAsync(string inputText);
}

public sealed record EventExtractionResult(bool Success, ExtractedEventData? Data, string? ErrorMessage);

public sealed record ExtractedEventData(
    string Name,
    string Description,
    string Link,
    DateTime StartDate,
    DateTime EndDate,
    int Classification,
    int Status,
    bool HasFees,
    bool IsOnline,
    int? Organization,
    string ProgramName,
    string Format,
    bool SchoolBookable,
    int AgeMinimum,
    int AgeMaximum,
    int? Location,
    int? Contact,
    List<int> TargetAudiences,
    List<int> Topics);
