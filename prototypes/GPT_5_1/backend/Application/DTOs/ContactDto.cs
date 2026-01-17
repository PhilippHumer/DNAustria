namespace Application.DTOs;

public class ContactDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? OrganizationId { get; set; }
}
