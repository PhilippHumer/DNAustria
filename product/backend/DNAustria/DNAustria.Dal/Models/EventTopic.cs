using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class EventTopic
{
    public int Event { get; set; }

    public int Topic { get; set; }

    public virtual Event EventNavigation { get; set; } = null!;
}
