using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Address { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public virtual Address? AddressNavigation { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
