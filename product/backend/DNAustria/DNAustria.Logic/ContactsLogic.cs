using DNAustria.Dal.Data;
using DNAustria.Domain;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Logic;

public class ContactsLogic(AppDbContext context) : IContactsLogic
{
    private readonly DbSet<Dal.Models.Contact> _contacts = context.Contacts;
    
    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        return await _contacts.Select(c => c.toDomain()).ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(int id)
    {
        var contact = await _contacts.FindAsync(id);
        return contact?.toDomain() ?? null;
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        var created = await _contacts.AddAsync(contact.ToEntity());
        await context.SaveChangesAsync();
        return created.Entity.toDomain();
    }

    public async Task<Contact> UpdateAsync(Contact contact)
    {
        var existing = await _contacts.FindAsync(contact.Id);
        if (existing is null) throw new NotFoundException($"Contact with id {contact.Id} not found");
        
        existing.Name = contact.Name;
        existing.Email = contact.Email;
        existing.Phone = contact.Phone;
        
        await context.SaveChangesAsync();
        return existing.toDomain();
    }

    public Task DeleteAsync(int id)
    {
        var existing = _contacts.Find(id);
        if (existing is null) throw new NotFoundException($"Contact with id {id} not found");
        
        _contacts.Remove(existing);
        return context.SaveChangesAsync();
    }
}