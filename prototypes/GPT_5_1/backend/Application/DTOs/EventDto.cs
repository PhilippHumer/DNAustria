using Domain.Entities;

namespace Application.DTOs;

public record EventDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int[]? Topics { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ContactId { get; set; }
    public int[]? TargetAudience { get; set; }
    public bool IsOnline { get; set; }
    public string? EventLink { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
}

