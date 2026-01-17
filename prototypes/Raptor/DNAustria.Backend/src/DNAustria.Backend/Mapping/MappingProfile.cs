using AutoMapper;
using DNAustria.Backend.Dtos;
using DNAustria.Backend.Models;

namespace DNAustria.Backend.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventDetailDto>().ReverseMap();
        CreateMap<EventCreateDto, Event>();
        CreateMap<AddressCreateDto, Address>();
        CreateMap<ContactCreateDto, Contact>();
        CreateMap<Address, Dtos.AddressCreateDto>().ReverseMap();
        CreateMap<Contact, Dtos.ContactCreateDto>().ReverseMap();
    }
}