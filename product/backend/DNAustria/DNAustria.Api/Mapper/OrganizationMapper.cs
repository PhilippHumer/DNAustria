using DNAustria.Api.Dtos;
using DNAustria.Api.Dtos.Address;
using DNAustria.Domain;
using Riok.Mapperly.Abstractions;

namespace DNAustria.Api.Mapper;

[Mapper]
public static partial class OrganizationMapper
{
    [MapProperty(nameof(Organization.Adress), 
        nameof(OrganizationDto.Adress))]
    public static partial OrganizationDto ToDto(this Organization organization);
    
    public static partial Organization ToDomain(this OrganizationDto organization);
    
    [MapperIgnoreTarget(nameof(Organization.Id))]
    [MapProperty(nameof(CreateOrganizationDto.Address), 
        nameof(Organization.Adress))]
    public static partial Organization ToDomain(this CreateOrganizationDto dto);
}