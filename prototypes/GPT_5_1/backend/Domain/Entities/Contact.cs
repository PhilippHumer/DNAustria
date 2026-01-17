using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
}

