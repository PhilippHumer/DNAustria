using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class Address
{
    public int Id { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Zip { get; set; } = null!;

    public string State { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
