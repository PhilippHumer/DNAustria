namespace DNAustria.Api.Dtos.Contacts;

public class ContactDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Organization { get; set; }
}

