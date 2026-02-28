using DNAustria.Dal.Models;
using Riok.Mapperly.Abstractions;

namespace DNAustria.Logic;

[Mapper]
public static partial class ContactsMapper
{
    //entity to domain
    [MapperIgnoreSource(nameof(contact.Events))]
    public static partial Domain.Contact toDomain(this Contact contact);
    
    //domain to entity
    public static partial Contact ToEntity(this Domain.Contact contact);
}