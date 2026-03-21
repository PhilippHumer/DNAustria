namespace DNAustria.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? OrganizationId { get; set; }
    public string? Org { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
}
