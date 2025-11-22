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
    [Route("server/api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EventsController(/*AppDbContext db*/) {/*_db = db;*/ }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(string? filter = null, string? search = null, int page = 1, int pageSize = 20)
        {
            var query = _db.Events.AsQueryable();
            // Simple search implementation for demo
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Title.Contains(search));
            // Filtering and paging
            var result = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Topics = e.Topics,
                    DateStart = e.DateStart,
                    DateEnd = e.DateEnd,
                    OrganizationId = e.OrganizationId,
                    ContactId = e.ContactId,
                    TargetAudience = e.TargetAudience,
                    IsOnline = e.IsOnline,
                    EventLink = e.EventLink,
                    Status = e.Status.ToString(),
                    CreatedBy = e.CreatedBy,
                    ModifiedBy = e.ModifiedBy,
                    ModifiedAt = e.ModifiedAt
                }).ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(Guid id)
        {
            var e = await _db.Events.FindAsync(id);
            if (e == null) return NotFound();
            return Ok(new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Topics = e.Topics,
                DateStart = e.DateStart,
                DateEnd = e.DateEnd,
                OrganizationId = e.OrganizationId,
                ContactId = e.ContactId,
                TargetAudience = e.TargetAudience,
                IsOnline = e.IsOnline,
                EventLink = e.EventLink,
                Status = e.Status.ToString(),
                CreatedBy = e.CreatedBy,
                ModifiedBy = e.ModifiedBy,
                ModifiedAt = e.ModifiedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] EventDto dto)
        {
            var model = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Topics = dto.Topics,
                DateStart = dto.DateStart,
                DateEnd = dto.DateEnd,
                OrganizationId = dto.OrganizationId,
                ContactId = dto.ContactId,
                TargetAudience = dto.TargetAudience,
                IsOnline = dto.IsOnline,
                EventLink = dto.EventLink,
                Status = Enum.TryParse<EventStatus>(dto.Status, out var status) ? status : EventStatus.Draft,
                CreatedBy = dto.CreatedBy,
                ModifiedBy = dto.ModifiedBy,
                ModifiedAt = DateTime.UtcNow,
            };
            _db.Events.Add(model);
            await _db.SaveChangesAsync();
            dto.Id = model.Id;
            dto.ModifiedAt = model.ModifiedAt;
            return CreatedAtAction(nameof(GetEvent), new { id = model.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] EventDto dto)
        {
            var e = await _db.Events.FindAsync(id);
            if (e == null) return NotFound();
            // Update fields
            e.Title = dto.Title;
            e.Description = dto.Description;
            e.Topics = dto.Topics;
            e.DateStart = dto.DateStart;
            e.DateEnd = dto.DateEnd;
            e.OrganizationId = dto.OrganizationId;
            e.ContactId = dto.ContactId;
            e.TargetAudience = dto.TargetAudience;
            e.IsOnline = dto.IsOnline;
            e.EventLink = dto.EventLink;
            e.Status = Enum.TryParse<EventStatus>(dto.Status, out var status) ? status : EventStatus.Draft;
            e.ModifiedBy = dto.ModifiedBy;
            e.ModifiedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var e = await _db.Events.FindAsync(id);
            if (e == null) return NotFound();
            _db.Events.Remove(e);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

