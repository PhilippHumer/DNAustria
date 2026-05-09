namespace DNAustria.Application.DTOs;

public record CreateAddressRequest(
    string? LocationName,
    string? Street,
    string? City,
    string? Zip,
    string? State,
    decimal? Latitude,
    decimal? Longitude
);
