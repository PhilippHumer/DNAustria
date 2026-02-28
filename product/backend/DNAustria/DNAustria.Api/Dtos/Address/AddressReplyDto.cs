namespace DNAustria.Api.Dtos.Address;

public class AddressReplyDto
{
    public int Id { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Zip { get; set; } = null!;

    public string State { get; set; } = null!;
}