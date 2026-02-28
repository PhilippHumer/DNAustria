using DNAustria.Api.Dtos.Address;

namespace DNAustria.Api.Dtos;

public record OrganizationDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required AddressDto Adress { get; set; }
}