using DNAustria.Domain;

namespace DNAustria.Logic.LocationsService;

public static class LocationMapperExtensions
{
    public static Location ToDomainLocation(this Dal.Models.Location location)
    {
        return new Location
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Address = location.AddressNavigation?.ToAddress() ?? null
        };
    }

    public static Dal.Models.Location ToDalLocation(this Location location)
    {
        return new Dal.Models.Location
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };
    }

    public static Dal.Models.Address ToDalAddress(this Address address)
    {
        return new Dal.Models.Address
        {
            Id = address.Id,
            State = address.State,
            City = address.City,
            Street = address.Street,
            Zip = address.Zip,
        };
    }

    public static Address ToAddress(this Dal.Models.Address address)
    {
        return new Address
        {
            Id = address.Id,
            State = address.State,
            City = address.City,
            Street = address.Street,
            Zip = address.Zip,
        };
    }
}