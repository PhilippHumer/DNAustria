using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class Organization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Adress { get; set; }

    public virtual Address? AdressNavigation { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
