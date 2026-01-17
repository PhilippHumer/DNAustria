using System.ComponentModel.DataAnnotations;

namespace DNAustria.Backend.Models;

public class Organization
{
    [Key]
    public Guid Id { get; set; }
    // Organization name (equivalent to Address.LocationName in previous model)
    public string Name { get; set; } = string.Empty;

    // Location fields similar to Address model
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Keep free-form address string for convenience
    public string Address { get; set; } = string.Empty;
}
