namespace DNAustria.Application.DTOs;

public record OrganizationDto(
    Guid Id,
    string Name,
    Guid? AddressId,
    DateTime CreatedAt,
    DateTime ModifiedAt);
