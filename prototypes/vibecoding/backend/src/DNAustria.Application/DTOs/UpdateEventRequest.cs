namespace DNAustria.Application.DTOs;

public record UpdateEventRequest(
    string? Title,
    string? Description,
    string? EventLink,
    List<int>? TargetAudience,
    List<int>? Topics,
    DateTime? DateStart,
    DateTime? DateEnd,
    string? Classification,
    bool? Fees,
    bool? IsOnline,
    Guid? OrganizationId,
    string? ProgramName,
    string? Format,
    bool? SchoolBookable,
    int? AgeMinimum,
    int? AgeMaximum,
    Guid? LocationId,
    Guid? ContactId,
    string? ModifiedBy
);
