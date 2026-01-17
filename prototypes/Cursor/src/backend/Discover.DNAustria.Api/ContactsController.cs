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
    [Route("server/api/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ContactsController(/*AppDbContext db*/) { /*_db = db;*/ }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            var result = await _db.Contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                OrganizationId = c.OrganizationId
            }).ToListAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContact(Guid id)
        {
            var c = await _db.Contacts.FindAsync(id);
            if (c == null) return NotFound();
            return Ok(new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                OrganizationId = c.OrganizationId
            });
        }
        [HttpPost]
        public async Task<ActionResult<ContactDto>> CreateContact([FromBody] ContactDto dto)
        {
            var model = new Contact
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                OrganizationId = dto.OrganizationId
            };
            _db.Contacts.Add(model);
            await _db.SaveChangesAsync();
            dto.Id = model.Id;
            return CreatedAtAction(nameof(GetContact), new { id = model.Id }, dto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateContact(Guid id, [FromBody] ContactDto dto)
        {
            var c = await _db.Contacts.FindAsync(id);
            if (c == null) return NotFound();
            c.Name = dto.Name;
            c.Email = dto.Email;
            c.Phone = dto.Phone;
            c.OrganizationId = dto.OrganizationId;
            await _db.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContact(Guid id)
        {
            var c = await _db.Contacts.FindAsync(id);
            if (c == null) return NotFound();
            _db.Contacts.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

