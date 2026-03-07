using DNAustria.Api.Dtos;
using DNAustria.Api.Dtos.Events;
using DNAustria.Domain;
using DNAustria.Logic.Events;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
public class EventsController(IEventLogic eventLogic) : ControllerBase
{
    private readonly IEventLogic _eventLogic = eventLogic ?? throw new ArgumentNullException(nameof(eventLogic));

    [HttpGet("api/events")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll([FromQuery] string? name)
    {
        var events = await _eventLogic.GetAllAsync(name);
        var dtos = events.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpGet("api/events/{id:int}")]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var e = await _eventLogic.GetByIdAsync(id);
        if (e is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(e));
    }

    [HttpPost("api/events")]
    public async Task<ActionResult<EventDto>> Create([FromBody] InsertEventDto req)
    {
        var domain = new Event(
            name: req.Name,
            description: req.Description,
            link: req.Link,
            startDate: req.StartDate,
            endDate: req.EndDate,
            classification: (EventClassification)req.Classification,
            status: (EventStatus)req.Status,
            hasFees: req.HasFees,
            isOnline: req.IsOnline,
            programName: req.ProgramName,
            format: req.Format,
            schoolBookable: req.SchoolBookable,
            ageMinimum: req.AgeMinimum,
            ageMaximum: req.AgeMaximum,
            organizationId: req.Organization,
            locationId: req.Location,
            contactId: req.Contact
        );

        var created = await _eventLogic.CreateAsync(
            domain,
            targetAudiences: req.TargetAudiences,
            topics: req.Topics);

        var dto = MapToDto(created);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            dto);
    }

    [HttpPut("api/events/{id:int}")]
    public async Task<ActionResult<EventDto>> Update(int id, [FromBody] UpdateEventDto req)
    {
        if (!Enum.IsDefined(typeof(EventStatus), req.Status))
        {
            return BadRequest($"Invalid status value: {req.Status}");
        }
        
        var domain = new Event(
            name: req.Name,
            description: req.Description,
            link: req.Link,
            startDate: req.StartDate,
            endDate: req.EndDate,
            classification: (EventClassification)req.Classification,
            status: (EventStatus)req.Status,
            hasFees: req.HasFees,
            isOnline: req.IsOnline,
            programName: req.ProgramName,
            format: req.Format,
            schoolBookable: req.SchoolBookable,
            ageMinimum: req.AgeMinimum,
            ageMaximum: req.AgeMaximum,
            organizationId: req.Organization,
            locationId: req.Location,
            contactId: req.Contact
        );

        var updated = await _eventLogic.UpdateAsync(
            id,
            domain,
            targetAudiences: req.TargetAudiences,
            topics: req.Topics);

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(updated));
    }

    [HttpDelete("api/events/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _eventLogic.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("api/events/{id:int}/status")]
    public async Task<ActionResult<EventDto>> UpdateStatus(int id, [FromBody] UpdateEventStatusDto req)
    {
        if (!Enum.IsDefined(typeof(EventStatus), req.Status))
        {
            return BadRequest($"Invalid status value: {req.Status}");
        }
        
        var updated = await _eventLogic.UpdateStatusAsync(id, req.Status);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(updated));
    }

    private static EventDto MapToDto(Event e)
    {
        return new EventDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Link = e.Link,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Classification = (int)e.Classification,
            Status = (int)e.Status,
            HasFees = e.HasFees,
            IsOnline = e.IsOnline,
            Organization = e.OrganizationId,
            ProgramName = e.ProgramName,
            Format = e.Format,
            SchoolBookable = e.SchoolBookable,
            AgeMinimum = e.AgeMinimum,
            AgeMaximum = e.AgeMaximum,
            Location = e.LocationId,
            Contact = e.ContactId,
            TargetAudiences = e.TargetAudiences.Select(x => (int)x.TargetAudience).ToList(),
            Topics = e.Topics.Select(x => (int)x.Topic).ToList()
        };
    }
}