using DNAustria.Domain.Enums;

namespace DNAustria.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EventLink { get; set; }
    public List<int> TargetAudience { get; set; } = [];
    public List<int> Topics { get; set; } = [];
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public Classification Classification { get; set; }
    public bool Fees { get; set; }
    public bool IsOnline { get; set; }
    public Guid OrganizationId { get; set; }
    public string? ProgramName { get; set; }
    public string? Format { get; set; }
    public bool? SchoolBookable { get; set; }
    public int? AgeMinimum { get; set; }
    public int? AgeMaximum { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ContactId { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
