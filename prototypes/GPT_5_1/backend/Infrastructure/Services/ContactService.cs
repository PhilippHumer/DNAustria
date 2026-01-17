using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ContactService : IContactService
{
    private readonly IAppDbContext _ctx; private readonly IMapper _mapper;
    public ContactService(IAppDbContext ctx, IMapper mapper){_ctx=ctx;_mapper=mapper;}

    public async Task<IEnumerable<ContactDto>> ListAsync()
    {
        var contacts = await _ctx.Contacts.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<ContactDto>>(contacts);
    }

    public async Task<ContactDto> CreateAsync(ContactDto dto)
    {
        var entity = _mapper.Map<Contact>(dto);
        entity.Id = Guid.NewGuid();
        _ctx.Contacts.Add(entity);
        await _ctx.SaveChangesAsync();
        return _mapper.Map<ContactDto>(entity);
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, ContactDto dto)
    {
        var entity = await _ctx.Contacts.FirstOrDefaultAsync(c=>c.Id==id);
        if(entity==null) return null;
        _mapper.Map(dto, entity);
        await _ctx.SaveChangesAsync();
        return _mapper.Map<ContactDto>(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _ctx.Contacts.FirstOrDefaultAsync(c=>c.Id==id);
        if(entity==null) return false;
        _ctx.Contacts.Remove(entity);
        await _ctx.SaveChangesAsync();
        return true;
    }
}
