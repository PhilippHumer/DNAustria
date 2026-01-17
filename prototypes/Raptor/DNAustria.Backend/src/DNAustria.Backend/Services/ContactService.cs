using DNAustria.Backend.Data;
using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Backend.Services;

public class ContactService : IContactService
{
    private readonly AppDbContext _db;
    public ContactService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Contact>> GetAllAsync() => await _db.Contacts.ToListAsync();

    public async Task<Contact?> GetAsync(Guid id) => await _db.Contacts.FindAsync(id);

    public async Task<Contact> CreateAsync(Contact c)
    {
        c.Id = Guid.NewGuid();
        _db.Contacts.Add(c);
        await _db.SaveChangesAsync();
        return c;
    }

    public async Task<Contact?> UpdateAsync(Guid id, Contact c)
    {
        var ex = await _db.Contacts.FindAsync(id);
        if (ex is null) return null;
        ex.Name = c.Name;
        ex.Org = c.Org;
        ex.Email = c.Email;
        ex.Phone = c.Phone;
        await _db.SaveChangesAsync();
        return ex;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ex = await _db.Contacts.FindAsync(id);
        if (ex is null) return false;
        _db.Contacts.Remove(ex);
        await _db.SaveChangesAsync();
        return true;
    }
}