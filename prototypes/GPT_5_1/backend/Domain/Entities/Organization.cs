using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? AddressStreet { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressZip { get; set; }
    public int? RegionId { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
