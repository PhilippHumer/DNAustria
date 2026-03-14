using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class User
{
    public int Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public virtual ICollection<EventHistory> EventHistories { get; set; } = new List<EventHistory>();
}
