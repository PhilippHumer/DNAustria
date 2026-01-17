namespace Application.DTOs;

public class OrganizationDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AddressStreet { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressZip { get; set; }
    public int? RegionId { get; set; }
}

