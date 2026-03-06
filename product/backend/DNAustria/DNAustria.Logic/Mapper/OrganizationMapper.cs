using Riok.Mapperly.Abstractions;

namespace DNAustria.Logic.Mapper;

[Mapper]
[UseStaticMapper(typeof(AddressMapper))]
public static partial class OrganizationMapper
{
    [MapperIgnoreSource(nameof(entity.Events))]
    [MapperIgnoreSource(nameof(entity.Adress))]
    [MapProperty(nameof(Dal.Models.Organization.AdressNavigation), 
        nameof(Domain.Organization.Adress))] 
    public static partial Domain.Organization ToDomain(this Dal.Models.Organization entity);

    
    
    [MapperIgnoreTarget(nameof(Dal.Models.Organization.Events))]
    [MapperIgnoreTarget(nameof(Dal.Models.Organization.AdressNavigation))]
    [MapProperty(nameof(Domain.Organization.Adress.Id), nameof(Dal.Models.Organization.Adress))]
    public static partial Dal.Models.Organization ToEntity(this Domain.Organization domain);
}