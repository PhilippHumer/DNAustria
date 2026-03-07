namespace DNAustria.Domain;

using System;
using System.Net.Mail;

public record Contact
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Organization { get; init; }

    // Geschützter parameterloser Konstruktor für ORMs (z.B. EF Core)
    protected Contact() { Name = string.Empty; }

    // Hauptkonstruktor mit Validierung - wirft Ausnahmen bei ungültiger Initialisierung
    public Contact(string name, string? email = null, string? phone = null, string? organization = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name darf nicht leer sein.", nameof(name));
        
        if (phone != null && string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone darf nicht nur aus Leerzeichen bestehen.", nameof(phone));

        Name = name.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Organization = string.IsNullOrWhiteSpace(organization) ? null : organization.Trim();
    }
    
    
}
