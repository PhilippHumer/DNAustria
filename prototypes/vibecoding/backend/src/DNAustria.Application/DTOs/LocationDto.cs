namespace DNAustria.Application.DTOs;

public record LocationDto(
    Guid Id,
    string Name,
    AddressDto Address,
    DateTime CreatedAt,
    DateTime ModifiedAt
);
