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
        CreateMap<OrganizationCreateDto, Organization>();
        CreateMap<ContactCreateDto, Contact>();
        CreateMap<Organization, Dtos.OrganizationCreateDto>().ReverseMap();
        CreateMap<Contact, Dtos.ContactCreateDto>().ReverseMap();
    }
}