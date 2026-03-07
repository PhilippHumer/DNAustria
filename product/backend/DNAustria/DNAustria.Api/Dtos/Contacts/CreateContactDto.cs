using System.ComponentModel.DataAnnotations;

namespace DNAustria.Api.Dtos.Contacts;

public class CreateContactDto
{
    [MaxLength(50)]
    public required string Name { get; set; } = null!;
    [EmailAddress]
    public string? Email { get; set; } 
    [Phone]
    public string? Phone { get; set; }
    public required string? Organization { get; set; }
}

