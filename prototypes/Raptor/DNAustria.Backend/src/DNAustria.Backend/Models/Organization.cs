using System.ComponentModel.DataAnnotations;

namespace DNAustria.Backend.Models;

public class Organization
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
