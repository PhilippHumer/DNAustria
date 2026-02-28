using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class EventTargetAudience
{
    public int Event { get; set; }

    public int TargetAudience { get; set; }

    public virtual Event EventNavigation { get; set; } = null!;
}
