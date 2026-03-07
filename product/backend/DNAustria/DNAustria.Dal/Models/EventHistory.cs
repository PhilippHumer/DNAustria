using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class EventHistory
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
