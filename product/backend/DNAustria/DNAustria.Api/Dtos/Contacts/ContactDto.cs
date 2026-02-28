namespace DNAustria.Api.Dtos.Contacts;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Organization { get; set; }
}

