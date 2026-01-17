using DNAustria.API.Data;
using DNAustria.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.API.Services;

public class ContactService
{
    private readonly AppDbContext _context;

    public ContactService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Contact> GetOrCreateContactAsync(Contact contact)
    {
        if (!string.IsNullOrEmpty(contact.Email))
        {
            var existing = await _context.Contacts.FirstOrDefaultAsync(c => c.Email == contact.Email);
            if (existing != null) return existing;
        }
        
        // Fallback: Name and Phone
        var existingByName = await _context.Contacts
            .FirstOrDefaultAsync(c => c.Name == contact.Name && c.Phone == contact.Phone);
            
        if (existingByName != null) return existingByName;

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }
}
