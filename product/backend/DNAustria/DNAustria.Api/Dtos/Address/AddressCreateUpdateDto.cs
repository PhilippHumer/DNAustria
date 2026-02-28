using System.ComponentModel.DataAnnotations;

namespace DNAustria.Api.Dtos.Address;

public class AddressCreateUpdateDto
{
    [Required]
    [MaxLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Zip { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [RegularExpression("^(Burgenland|Kärnten|Niederösterreich|Oberösterreich|Salzburg|Steiermark|Tirol|Vorarlberg|Wien)$",
    ErrorMessage = "Invalid state")]
    public string State { get; set; } = string.Empty;
}