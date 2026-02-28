namespace DNAustria.Api.Dtos.Events;

public record EventDto(
    int Id,
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
    IReadOnlyList<int> TargetAudiences,
    IReadOnlyList<int> Topics
);