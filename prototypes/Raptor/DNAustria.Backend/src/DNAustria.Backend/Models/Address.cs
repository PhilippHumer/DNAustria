using System.ComponentModel.DataAnnotations;

namespace DNAustria.Backend.Models;

public class Address
{
    [Key]
    public Guid Id { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
