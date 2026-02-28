namespace DNAustria.Domain;

public class Location
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public required Address? Address { get; set; }
}