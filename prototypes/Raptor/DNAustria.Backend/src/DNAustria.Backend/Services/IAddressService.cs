using DNAustria.Backend.Models;

namespace DNAustria.Backend.Services;

public interface IAddressService
{
    Task<IEnumerable<Address>> GetAllAsync();
    Task<Address?> GetAsync(Guid id);
    Task<Address> CreateAsync(Address a);
    Task<Address?> UpdateAsync(Guid id, Address a);
    Task<bool> DeleteAsync(Guid id);
}