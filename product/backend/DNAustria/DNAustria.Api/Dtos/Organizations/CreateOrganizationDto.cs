using DNAustria.Api.Dtos.Address;

namespace DNAustria.Api.Dtos;

public record CreateOrganizationDto()
{
    public required string Name { get; set; }
    public required AddressDto Address { get; set; }
}