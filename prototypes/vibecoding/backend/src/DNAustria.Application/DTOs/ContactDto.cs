namespace DNAustria.Application.DTOs;

public record ContactDto(
    Guid Id,
    string Name,
    Guid? OrganizationId,
    string? Org,
    string Email,
    string Phone,
    DateTime CreatedAt,
    DateTime? ModifiedAt
);
