using DNAustria.Api.Dtos.Contacts;
using DNAustria.Domain;
using Riok.Mapperly.Abstractions;

namespace DNAustria.Api;

[Mapper]
public static partial class ContactsMapper
{
        //domain to dto
        public static partial ContactDto ToDto(this Contact contact);
        public static partial IEnumerable<ContactDto> ToDtoCollection(this IEnumerable<Contact> contacts);
        
        //dto to domain
        public static partial Contact ToDomain(this CreateContactDto dto);
        public static partial Contact ToDomain(this UpdateContactDto dto);
        
}