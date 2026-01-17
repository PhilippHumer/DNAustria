using DNAustria.Backend.Data;
using DNAustria.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Backend.Services;

public class AddressService : IAddressService
{
    private readonly AppDbContext _db;
    public AddressService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Address>> GetAllAsync() => await _db.Addresses.ToListAsync();

    public async Task<Address?> GetAsync(Guid id) => await _db.Addresses.FindAsync(id);

    public async Task<Address> CreateAsync(Address a)
    {
        a.Id = Guid.NewGuid();
        _db.Addresses.Add(a);
        await _db.SaveChangesAsync();
        return a;
    }

    public async Task<Address?> UpdateAsync(Guid id, Address a)
    {
        var ex = await _db.Addresses.FindAsync(id);
        if (ex is null) return null;
        ex.LocationName = a.LocationName;
        ex.City = a.City;
        ex.Zip = a.Zip;
        ex.State = a.State;
        ex.Street = a.Street;
        ex.Latitude = a.Latitude;
        ex.Longitude = a.Longitude;
        await _db.SaveChangesAsync();
        return ex;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ex = await _db.Addresses.FindAsync(id);
        if (ex is null) return false;
        _db.Addresses.Remove(ex);
        await _db.SaveChangesAsync();
        return true;
    }
}