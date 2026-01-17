using System.ComponentModel.DataAnnotations;

namespace DNAustria.API.Models;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EventLink { get; set; } = string.Empty;
    
    public List<TargetAudience> TargetAudience { get; set; } = new();
    public List<EventTopic> Topics { get; set; } = new();
    
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    
    public EventClassification Classification { get; set; }
    public bool Fees { get; set; }
    public bool IsOnline { get; set; }
    
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    
    public string? ProgramName { get; set; }
    public string? Format { get; set; }
    public bool? SchoolBookable { get; set; }
    public int? AgeMinimum { get; set; }
    public int? AgeMaximum { get; set; }
    
    public Guid? LocationId { get; set; }
    public Address? Location { get; set; }
    
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }
    
    public EventStatus Status { get; set; }
    
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
}
