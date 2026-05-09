namespace DNAustria.Application.DTOs;

public record CreateOrganizationRequest(string? Name, Guid? AddressId);
