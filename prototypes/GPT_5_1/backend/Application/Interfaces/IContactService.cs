using Application.DTOs;

namespace Application.Interfaces;

public interface IContactService
{
    Task<IEnumerable<ContactDto>> ListAsync();
    Task<ContactDto> CreateAsync(ContactDto dto);
    Task<ContactDto?> UpdateAsync(Guid id, ContactDto dto);
    Task<bool> DeleteAsync(Guid id);
}
