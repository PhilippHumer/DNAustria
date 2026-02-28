namespace DNAustria.Api.Dtos.Address;

public record AddressDto
{
    public int Id { get; set; }
    public required string State { get; set; }
    public required string Zip { get; set; }
    public required string City { get; set; }
    public required string Street { get; set; }
}