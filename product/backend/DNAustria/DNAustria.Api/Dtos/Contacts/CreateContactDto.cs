using System.ComponentModel.DataAnnotations;

namespace DNAustria.Api.Dtos.Contacts;

public class CreateContactDto
{
    [MaxLength(50)]
    public required string Name { get; set; } = null!;
    [EmailAddress]
    [RegularExpression(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$",
        ErrorMessage = "Invalid email address format.")]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
    public required string? Organization { get; set; }
}

