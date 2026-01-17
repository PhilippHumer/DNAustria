using DNAustria.API.Data;
using DNAustria.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.API.Services;

public class AddressService
{
    private readonly AppDbContext _context;

    public AddressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Address> GetOrCreateAddressAsync(Address address)
    {
        // Check if address exists matching Street, Zip, City
        var existing = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Zip == address.Zip && 
                                      a.City.ToLower() == address.City.ToLower() &&
                                      a.Street.ToLower() == address.Street.ToLower());

        if (existing != null)
        {
            return existing;
        }

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return address;
    }
}
