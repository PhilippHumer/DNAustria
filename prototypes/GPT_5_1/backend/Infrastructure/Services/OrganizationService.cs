using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IAppDbContext _ctx; private readonly IMapper _mapper;
    public OrganizationService(IAppDbContext ctx, IMapper mapper){_ctx=ctx;_mapper=mapper;}

    public async Task<IEnumerable<OrganizationDto>> ListAsync()
    {
        var orgs = await _ctx.Organizations.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<OrganizationDto>>(orgs);
    }

    public async Task<OrganizationDto> CreateAsync(OrganizationDto dto)
    {
        var entity = _mapper.Map<Organization>(dto);
        entity.Id = Guid.NewGuid();
        _ctx.Organizations.Add(entity);
        await _ctx.SaveChangesAsync();
        return _mapper.Map<OrganizationDto>(entity);
    }

    public async Task<OrganizationDto?> UpdateAsync(Guid id, OrganizationDto dto)
    {
        var entity = await _ctx.Organizations.FirstOrDefaultAsync(o=>o.Id==id);
        if(entity==null) return null;
        _mapper.Map(dto, entity);
        await _ctx.SaveChangesAsync();
        return _mapper.Map<OrganizationDto>(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _ctx.Organizations.FirstOrDefaultAsync(o=>o.Id==id);
        if(entity==null) return false;
        _ctx.Organizations.Remove(entity);
        await _ctx.SaveChangesAsync();
        return true;
    }
}
