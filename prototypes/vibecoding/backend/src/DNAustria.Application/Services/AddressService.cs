using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;

namespace DNAustria.Application.Services;

public class AddressService
{
    private readonly IAddressRepository _repository;

    public AddressService(IAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AddressDto>> GetAllAsync(CancellationToken ct = default)
    {
        var addresses = await _repository.GetAllAsync(ct);
        return addresses.Select(ToDto).ToList();
    }

    public async Task<AddressDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Address {id} not found.");
        return ToDto(address);
    }

    public async Task<AddressDto> CreateAsync(CreateAddressRequest request, CancellationToken ct = default)
    {
        var locationName = request.LocationName?.Trim() ?? string.Empty;
        var street = request.Street?.Trim() ?? string.Empty;
        var city = request.City?.Trim() ?? string.Empty;
        var zip = request.Zip?.Trim() ?? string.Empty;
        var state = request.State?.Trim() ?? string.Empty;

        ValidateFields(locationName, street, city, zip, state, request.Latitude, request.Longitude);

        var latitude = request.Latitude!.Value;
        var longitude = request.Longitude!.Value;

        var existing = await _repository.FindActiveDuplicateAsync(zip, latitude, longitude, ct);
        if (existing != null)
            return ToDto(existing);

        var address = new Address
        {
            Id = Guid.NewGuid(),
            LocationName = locationName,
            Street = street,
            City = city,
            Zip = zip,
            State = state,
            Latitude = latitude,
            Longitude = longitude,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(address, ct);
        await _repository.SaveChangesAsync(ct);

        return ToDto(address);
    }

    public async Task<AddressDto> UpdateAsync(Guid id, UpdateAddressRequest request, CancellationToken ct = default)
    {
        var address = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Address {id} not found.");

        var locationName = request.LocationName?.Trim() ?? string.Empty;
        var street = request.Street?.Trim() ?? string.Empty;
        var city = request.City?.Trim() ?? string.Empty;
        var zip = request.Zip?.Trim() ?? string.Empty;
        var state = request.State?.Trim() ?? string.Empty;

        ValidateFields(locationName, street, city, zip, state, request.Latitude, request.Longitude);

        var latitude = request.Latitude!.Value;
        var longitude = request.Longitude!.Value;

        if (await _repository.ExistsAnotherWithDedupKeyAsync(zip, latitude, longitude, id, ct))
            throw new ArgumentException($"Another active address with zip '{zip}', latitude '{latitude}', longitude '{longitude}' already exists.");

        address.LocationName = locationName;
        address.Street = street;
        address.City = city;
        address.Zip = zip;
        address.State = state;
        address.Latitude = latitude;
        address.Longitude = longitude;
        address.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);

        return ToDto(address);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Address {id} not found.");

        if (await _repository.HasActiveLocationsAsync(id, ct))
            throw new ArgumentException($"Address {id} is still referenced by one or more active locations.");

        address.IsDeleted = true;
        address.ModifiedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(ct);
    }

    private static void ValidateFields(string locationName, string street, string city, string zip, string state, decimal? latitude, decimal? longitude)
    {
        if (string.IsNullOrEmpty(locationName) || locationName.Length > 50)
            throw new ArgumentException("LocationName is required and must be at most 50 characters.");
        if (string.IsNullOrEmpty(street) || street.Length > 50)
            throw new ArgumentException("Street is required and must be at most 50 characters.");
        if (string.IsNullOrEmpty(city) || city.Length > 50)
            throw new ArgumentException("City is required and must be at most 50 characters.");
        if (string.IsNullOrEmpty(zip) || zip.Length > 10)
            throw new ArgumentException("Zip is required and must be at most 10 characters.");
        if (string.IsNullOrEmpty(state) || state.Length > 50)
            throw new ArgumentException("State is required and must be at most 50 characters.");
        if (latitude == null)
            throw new ArgumentException("Latitude is required.");
        if (longitude == null)
            throw new ArgumentException("Longitude is required.");
    }

    public static AddressDto ToDto(Address address) =>
        new(address.Id, address.LocationName, address.Street, address.City,
            address.Zip, address.State, address.Latitude, address.Longitude,
            address.CreatedAt, address.ModifiedAt);
}
