using DNAustria.Api.Dtos.Address;
using DNAustria.Api.Dtos.Contact;
using DNAustria.Api.Dtos.Locations;
using DNAustria.Domain;

namespace DNAustria.Api.MapperExtensions;

public static class LocationMapperExtensions
{
    public static LocationReplyDto ToLocationReplyDto(this Location location)
    {
        return new LocationReplyDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Address = location.Address?.ToAddressReplyDto() ?? null
        };
    }

    public static Location ToLocation(this CreateUpdateLocationDto request)
    {
        return new Location()
        {
            Name = request.Name,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address?.ToAddress() ?? null
        };
    }

    public static AddressReplyDto ToAddressReplyDto(this Address address)
    {
        return new AddressReplyDto
        {
            Id = address.Id,
            State = address.State,
            City = address.City,
            Zip = address.Zip,
            Street = address.Street,
        };
    }

    public static Address ToAddress(this AddressCreateUpdateDto createUpdate)
    {
        return new Address
        {
            State = createUpdate.State,
            City = createUpdate.City,
            Zip = createUpdate.Zip,
            Street = createUpdate.Street
        };
    }
}