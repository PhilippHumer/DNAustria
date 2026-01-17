using Application.DTOs;

namespace Application.Interfaces;

public interface IOrganizationService
{
    Task<IEnumerable<OrganizationDto>> ListAsync();
    Task<OrganizationDto> CreateAsync(OrganizationDto dto);
    Task<OrganizationDto?> UpdateAsync(Guid id, OrganizationDto dto);
    Task<bool> DeleteAsync(Guid id);
}
