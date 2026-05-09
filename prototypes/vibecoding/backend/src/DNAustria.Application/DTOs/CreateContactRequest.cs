namespace DNAustria.Application.DTOs;

public record CreateContactRequest(
    string? Name,
    string? Email,
    string? Phone,
    Guid? OrganizationId,
    string? Org
);
