namespace DNAustria.Logic.Mapper;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AddressMapper
{
    [MapperIgnoreSource(nameof(entity.Organizations))]
    [MapperIgnoreSource(nameof(entity.Locations))]
    [MapperIgnoreSource(nameof(entity.IsDeleted))]
    public static partial Domain.Address ToDomain(this Dal.Models.Address entity);

    [MapperIgnoreTarget(nameof(Dal.Models.Address.Organizations))]
    [MapperIgnoreTarget(nameof(Dal.Models.Address.Locations))]
    [MapperIgnoreTarget(nameof(Dal.Models.Address.IsDeleted))]
    public static partial Dal.Models.Address ToEntity(this Domain.Address domain);
}