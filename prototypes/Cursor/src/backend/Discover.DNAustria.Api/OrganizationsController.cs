using Discover.DNAustria.Domain;
using Discover.DNAustria.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discover.DNAustria.Api
{
    [ApiController]
    [Route("server/api/organizations")]
    public class OrganizationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OrganizationsController(/*AppDbContext db*/) {/* _db = db;*/ }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizations()
        {
            var result = await _db.Organizations.Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                AddressStreet = o.AddressStreet,
                AddressCity = o.AddressCity,
                AddressZip = o.AddressZip,
                RegionId = o.RegionId
            }).ToListAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDto>> GetOrganization(Guid id)
        {
            var o = await _db.Organizations.FindAsync(id);
            if (o == null) return NotFound();
            return Ok(new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                AddressStreet = o.AddressStreet,
                AddressCity = o.AddressCity,
                AddressZip = o.AddressZip,
                RegionId = o.RegionId
            });
        }
        [HttpPost]
        public async Task<ActionResult<OrganizationDto>> CreateOrganization([FromBody] OrganizationDto dto)
        {
            var model = new Organization
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                AddressStreet = dto.AddressStreet,
                AddressCity = dto.AddressCity,
                AddressZip = dto.AddressZip,
                RegionId = dto.RegionId
            };
            _db.Organizations.Add(model);
            await _db.SaveChangesAsync();
            dto.Id = model.Id;
            return CreatedAtAction(nameof(GetOrganization), new { id = model.Id }, dto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrganization(Guid id, [FromBody] OrganizationDto dto)
        {
            var o = await _db.Organizations.FindAsync(id);
            if (o == null) return NotFound();
            o.Name = dto.Name;
            o.AddressStreet = dto.AddressStreet;
            o.AddressCity = dto.AddressCity;
            o.AddressZip = dto.AddressZip;
            o.RegionId = dto.RegionId;
            await _db.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrganization(Guid id)
        {
            var o = await _db.Organizations.FindAsync(id);
            if (o == null) return NotFound();
            _db.Organizations.Remove(o);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

