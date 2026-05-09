using DNAustria.Application.DTOs;
using DNAustria.Application.Exceptions;
using DNAustria.Application.Interfaces;
using DNAustria.Domain.Entities;

namespace DNAustria.Application.Services;

public class LocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IAddressRepository _addressRepository;

    public LocationService(ILocationRepository locationRepository, IAddressRepository addressRepository)
    {
        _locationRepository = locationRepository;
        _addressRepository = addressRepository;
    }

    public async Task<List<LocationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var locations = await _locationRepository.GetAllAsync(ct);
        return locations.Select(ToDto).ToList();
    }

    public async Task<LocationDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var location = await _locationRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Location {id} not found.");
        return ToDto(location);
    }

    public async Task<LocationDto> CreateAsync(CreateLocationRequest request, CancellationToken ct = default)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new ArgumentException("Name is required and must be at most 50 characters.");

        if (request.AddressId == null || request.AddressId == Guid.Empty)
            throw new ArgumentException("AddressId is required.");

        var address = await _addressRepository.GetByIdAsync(request.AddressId.Value, ct)
            ?? throw new NotFoundException($"Address {request.AddressId} not found.");

        var location = new Location
        {
            Id = Guid.NewGuid(),
            Name = name,
            AddressId = address.Id,
            Address = address,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await _locationRepository.AddAsync(location, ct);
        await _locationRepository.SaveChangesAsync(ct);

        return ToDto(location);
    }

    public async Task<LocationDto> UpdateAsync(Guid id, UpdateLocationRequest request, CancellationToken ct = default)
    {
        var location = await _locationRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Location {id} not found.");

        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            throw new ArgumentException("Name is required and must be at most 50 characters.");

        if (request.AddressId == null || request.AddressId == Guid.Empty)
            throw new ArgumentException("AddressId is required.");

        var address = await _addressRepository.GetByIdAsync(request.AddressId.Value, ct)
            ?? throw new NotFoundException($"Address {request.AddressId} not found.");

        location.Name = name;
        location.AddressId = address.Id;
        location.Address = address;
        location.ModifiedAt = DateTime.UtcNow;

        await _locationRepository.SaveChangesAsync(ct);

        return ToDto(location);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var location = await _locationRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Location {id} not found.");

        location.IsDeleted = true;
        location.ModifiedAt = DateTime.UtcNow;

        await _locationRepository.SaveChangesAsync(ct);
    }

    private static LocationDto ToDto(Location location) =>
        new(location.Id, location.Name, AddressService.ToDto(location.Address),
            location.CreatedAt, location.ModifiedAt);
}
