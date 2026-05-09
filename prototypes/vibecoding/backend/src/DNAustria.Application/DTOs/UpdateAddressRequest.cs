namespace DNAustria.Application.DTOs;

public record UpdateAddressRequest(
    string? LocationName,
    string? Street,
    string? City,
    string? Zip,
    string? State,
    decimal? Latitude,
    decimal? Longitude
);
