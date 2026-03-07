using DNAustria.Api.Dtos.Address;
using DNAustria.Domain;
using Riok.Mapperly.Abstractions;

namespace DNAustria.Api.Mapper;

[Mapper]
public static partial class AddressMapper
{
    public static partial AddressDto ToDto(this Address address);
    public static partial Address ToDomain(this AddressDto dto);
}