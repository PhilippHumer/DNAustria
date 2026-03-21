namespace DNAustria.Application.DTOs;

public record UpdateContactRequest(
    string? Name,
    string? Email,
    string? Phone,
    Guid? OrganizationId,
    string? Org
);
