using System;
using System.Collections.Generic;

namespace DNAustria.Dal.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Organization { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
