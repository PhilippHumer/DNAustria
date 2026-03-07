using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class Event
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Link { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int Classification { get; set; }

    public int Status { get; set; }

    public bool HasFees { get; set; }

    public bool IsOnline { get; set; }

    public int? Organization { get; set; }

    public string ProgramName { get; set; } = null!;

    public string Format { get; set; } = null!;

    public bool SchoolBookable { get; set; }

    public int AgeMinimum { get; set; }

    public int AgeMaximum { get; set; }

    public int? Location { get; set; }

    public int? Contact { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Contact? ContactNavigation { get; set; }

    public virtual ICollection<EventHistory> EventHistories { get; set; } = new List<EventHistory>();

    public virtual ICollection<EventTargetAudience> EventTargetAudiences { get; set; } = new List<EventTargetAudience>();

    public virtual ICollection<EventTopic> EventTopics { get; set; } = new List<EventTopic>();

    public virtual Location? LocationNavigation { get; set; }

    public virtual Organization? OrganizationNavigation { get; set; }
}
