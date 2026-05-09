namespace DNAustria.Application.DTOs;

public record AddressDto(
    Guid Id,
    string LocationName,
    string Street,
    string City,
    string Zip,
    string State,
    decimal Latitude,
    decimal Longitude,
    DateTime CreatedAt,
    DateTime ModifiedAt
);
