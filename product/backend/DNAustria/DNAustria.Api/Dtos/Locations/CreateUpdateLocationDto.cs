using System.ComponentModel.DataAnnotations;
using DNAustria.Api.Dtos.Address;

namespace DNAustria.Api.Dtos.Locations;

public class CreateUpdateLocationDto
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }
    
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
    
    public AddressCreateUpdateDto? Address { get; set; }
}