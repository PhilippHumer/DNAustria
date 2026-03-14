using System.ComponentModel.DataAnnotations;

namespace DNAustria.Application.DTOs;

public class UpdateOrganizationRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public Guid? AddressId { get; set; }
}
