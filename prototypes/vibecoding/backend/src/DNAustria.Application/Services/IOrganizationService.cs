using DNAustria.Application.DTOs;

namespace DNAustria.Application.Services;

public interface IOrganizationService
{
    Task<IEnumerable<OrganizationDto>> GetAllAsync(string? nameFilter = null);
    Task<OrganizationDto> GetByIdAsync(Guid id);
    Task<OrganizationDto> CreateAsync(CreateOrganizationRequest request);
    Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationRequest request);
    Task DeleteAsync(Guid id);
}
