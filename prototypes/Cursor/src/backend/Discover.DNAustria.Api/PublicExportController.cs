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
    [Route("server/api/public/events")]
    public class PublicExportController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PublicExportController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DNAustriaExportEventDto>>> GetExportEvents()
        {
            var result = await _db.Events.Include(e => e.Organization).Include(e => e.Contact)
                .Where(e => e.Status == EventStatus.Approved || e.Status == EventStatus.Transferred)
                .Join(_db.Organizations,
                    e => e.OrganizationId,
                    o => o.Id,
                    (e, o) => new { e, o })
                .GroupJoin(_db.Contacts, x => x.e.ContactId, c => c.Id,
                    (xo, contacts) => new { xo, contact = contacts.FirstOrDefault() })
                .Select(x => new DNAustriaExportEventDto
                {
                    EventTitle = x.xo.e.Title,
                    EventDescription = x.xo.e.Description,
                    EventStart = x.xo.e.DateStart,
                    EventEnd = x.xo.e.DateEnd,
                    EventLink = x.xo.e.EventLink,
                    EventTopics = x.xo.e.Topics,
                    EventTargetAudience = x.xo.e.TargetAudience,
                    EventIsOnline = x.xo.e.IsOnline,
                    OrganizationName = x.xo.o.Name,
                    EventContactEmail = x.contact != null ? x.contact.Email : null,
                    EventContactPhone = x.contact != null ? x.contact.Phone : null,
                    EventAddressStreet = x.xo.o.AddressStreet,
                    EventAddressCity = x.xo.o.AddressCity,
                    EventAddressZip = x.xo.o.AddressZip,
                    EventAddressState = x.xo.o.RegionId != null ? x.xo.o.RegionId.ToString() : null,
                    Location = null // placeholder, geo not modeled
                })
                .ToListAsync();
            return Ok(result);
        }
    }
}

