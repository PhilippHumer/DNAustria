namespace DNAustria.Domain;

public class Organization
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required Address Adress { get; set; }
}