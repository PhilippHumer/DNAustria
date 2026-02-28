using DNAustria.Api.Dtos;
using DNAustria.Api.Dtos.Events;
using DNAustria.Logic.Events;
using DNAustria.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DNAustria.Api.Controllers;

[ApiController]
public class EventsController(IEventLogic eventLogic) : ControllerBase
{
    private readonly IEventLogic _eventLogic = eventLogic ?? throw new ArgumentNullException(nameof(eventLogic));

    private const int PublicStatus = 1;

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
        if (e is null) return NotFound();

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
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }
    
    [HttpPut("api/events/{id:int}")]
    public async Task<ActionResult<EventDto>> Update(int id, [FromBody] UpdateEventDto req)
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

        var updated = await _eventLogic.UpdateAsync(
            id,
            domain,
            targetAudiences: req.TargetAudiences,
            topics: req.Topics);

        if (updated is null) return NotFound();
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
        var updated = await _eventLogic.UpdateStatusAsync(id, req.Status);
        if (updated is null) return NotFound();

        return Ok(MapToDto(updated));
    }

    // -----------------------
    // Mapping: Domain -> DTO
    // -----------------------
    private static EventDto MapToDto(Event e)
    {
        return new EventDto
        (
            e.Id,
            e.Name,
            e.Description,
            e.Link,
            e.StartDate,
            e.EndDate,
            (int)e.Classification,
            (int)e.Status,
            e.HasFees,
            e.IsOnline,
            e.OrganizationId,
            e.ProgramName,
            e.Format,
            e.SchoolBookable,
            e.AgeMinimum,
            e.AgeMaximum,
            e.LocationId,
            e.ContactId,
            e.TargetAudiences.Select(x => x.TargetAudience).ToList(),
            e.Topics.Select(x => x.Topic).ToList()
        );
    }
}