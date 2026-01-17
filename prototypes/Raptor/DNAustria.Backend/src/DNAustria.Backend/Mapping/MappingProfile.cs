using AutoMapper;
using DNAustria.Backend.Dtos;
using DNAustria.Backend.Models;

namespace DNAustria.Backend.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventDetailDto>()
            .ForMember(d => d.Location, o => o.MapFrom(s =>
                // Prefer explicit scalar fields stored on the event, but if they are absent and the Organization navigation is loaded, use that instead.
                (s.LocationName != null || s.LocationStreet != null || s.LocationCity != null || s.LocationZip != null || s.LocationLatitude != null || s.LocationLongitude != null)
                    ? new OrganizationCreateDto
                    {
                        Name = s.LocationName ?? string.Empty,
                        Street = s.LocationStreet ?? string.Empty,
                        City = s.LocationCity ?? string.Empty,
                        Zip = s.LocationZip ?? string.Empty,
                        State = s.LocationState ?? string.Empty,
                        Latitude = s.LocationLatitude,
                        Longitude = s.LocationLongitude
                    }
                    : (s.Location != null ? new OrganizationCreateDto
                    {
                        Name = s.Location.Name ?? string.Empty,
                        Street = s.Location.Street ?? string.Empty,
                        City = s.Location.City ?? string.Empty,
                        Zip = s.Location.Zip ?? string.Empty,
                        State = s.Location.State ?? string.Empty,
                        Latitude = s.Location.Latitude,
                        Longitude = s.Location.Longitude
                    } : null)));


        // Do not auto-map Location object into Organization navigation. Service handles saving of the provided Location object into event fields.
        CreateMap<EventCreateDto, Event>()
            .ForMember(e => e.Location, opt => opt.Ignore())
            .ForMember(e => e.LocationId, opt => opt.Ignore())
            .ForMember(e => e.LocationName, opt => opt.Ignore())
            .ForMember(e => e.LocationStreet, opt => opt.Ignore())
            .ForMember(e => e.LocationCity, opt => opt.Ignore())
            .ForMember(e => e.LocationZip, opt => opt.Ignore())
            .ForMember(e => e.LocationState, opt => opt.Ignore())
            .ForMember(e => e.LocationLatitude, opt => opt.Ignore())
            .ForMember(e => e.LocationLongitude, opt => opt.Ignore());
        CreateMap<OrganizationCreateDto, Organization>();
        CreateMap<ContactCreateDto, Contact>();
        CreateMap<Organization, Dtos.OrganizationCreateDto>().ReverseMap();
        CreateMap<Contact, Dtos.ContactCreateDto>().ReverseMap();
    }
}