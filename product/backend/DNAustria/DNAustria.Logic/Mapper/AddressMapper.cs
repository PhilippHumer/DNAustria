namespace DNAustria.Logic.Mapper;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AddressMapper
{
    [MapperIgnoreSource(nameof(entity.Organizations))]
    [MapperIgnoreSource(nameof(entity.Locations))]
    public static partial Domain.Address ToDomain(this Dal.Models.Address entity);
}