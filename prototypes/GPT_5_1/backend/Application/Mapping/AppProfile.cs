using Application.DTOs;
using Domain.Entities;
using AutoMapper;

namespace Application.Mapping;

public class AppProfile : Profile
{
    public AppProfile()
    {
        CreateMap<Event, EventDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<Organization, OrganizationDto>().ReverseMap();
        CreateMap<Event, ExportEventDto>()
            .ForMember(d => d.EventTitle, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.EventDescription, o => o.MapFrom(s => s.Description))
            .ForMember(d => d.EventStart, o => o.MapFrom(s => s.DateStart))
            .ForMember(d => d.EventEnd, o => o.MapFrom(s => s.DateEnd))
            .ForMember(d => d.EventLink, o => o.MapFrom(s => s.EventLink))
            .ForMember(d => d.EventTopics, o => o.MapFrom(s => s.Topics))
            .ForMember(d => d.EventTargetAudience, o => o.MapFrom(s => s.TargetAudience))
            .ForMember(d => d.EventIsOnline, o => o.MapFrom(s => s.IsOnline))
            .ForMember(d => d.OrganizationName, o => o.MapFrom(s => s.Organization != null ? s.Organization.Name : null))
            .ForMember(d => d.EventContactEmail, o => o.MapFrom(s => s.Contact != null ? s.Contact.Email : null))
            .ForMember(d => d.EventContactPhone, o => o.MapFrom(s => s.Contact != null ? s.Contact.Phone : null))
            .ForMember(d => d.EventAddressStreet, o => o.MapFrom(s => s.Organization != null ? s.Organization.AddressStreet : null))
            .ForMember(d => d.EventAddressCity, o => o.MapFrom(s => s.Organization != null ? s.Organization.AddressCity : null))
            .ForMember(d => d.EventAddressZip, o => o.MapFrom(s => s.Organization != null ? s.Organization.AddressZip : null))
            .ForMember(d => d.EventAddressState, o => o.MapFrom(s => s.Organization != null ? s.Organization.RegionId.ToString() : null))
            .ForMember(d => d.Location, o => o.Ignore());
    }
}

