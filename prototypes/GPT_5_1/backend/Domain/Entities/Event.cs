using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    // Als PostgreSQL integer[] gespeichert
    public int[]? Topics { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ContactId { get; set; }
    public int[]? TargetAudience { get; set; }
    public bool IsOnline { get; set; }
    public string? EventLink { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public Organization? Organization { get; set; }
    public Contact? Contact { get; set; }
}

