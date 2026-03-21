using DNAustria.Api.Dtos;
using DNAustria.Api.Dtos.Events;
using DNAustria.Domain;
using DNAustria.Logic;
using DNAustria.Logic.Events;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DNAustria.Api.Controllers;

[ApiController]
public class EventsController(IEventLogic eventLogic, ILLMLogic llmLogic, IConfiguration configuration) : ControllerBase
{
    private readonly IEventLogic _eventLogic = eventLogic ?? throw new ArgumentNullException(nameof(eventLogic));
    private readonly ILLMLogic _llmLogic = llmLogic ?? throw new ArgumentNullException(nameof(llmLogic));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));


    [HttpGet("api/events")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll([FromQuery] string? name)
    {
        var events = await _eventLogic.GetAllAsync(name);
        var dtos = events.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    [HttpPost("api/events/llm")]
    public async Task<ActionResult<EventDto>> PostLlm([FromBody] LlmRequestDto? req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.Prompt))
            return BadRequest("Prompt is required.");

        try
        {
            var llmText = await LLMLogic.StartAsync(_configuration, req.Prompt);
            if (string.IsNullOrWhiteSpace(llmText))
                return BadRequest("LLM returned empty response.");

            // try to extract JSON object from LLM response
            var start = llmText.IndexOf('{');
            var end = llmText.LastIndexOf('}');
            if (start < 0 || end <= start)
                return BadRequest("LLM did not return a JSON object. Response: " + llmText);

            var json = llmText[start..(end + 1)];

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            InsertEventDto? dto;
            try
            {
                dto = JsonSerializer.Deserialize<InsertEventDto>(json, options);
            }
            catch (JsonException)
            {
                return BadRequest("Failed to parse JSON from LLM response.");
            }

            if (dto is null)
                return BadRequest("LLM produced no event data.");

            // Map DTO to domain (same as Create endpoint)
            var domain = new Event(
                name: dto.Name,
                description: dto.Description,
                link: dto.Link,
                startDate: dto.StartDate,
                endDate: dto.EndDate,
                classification: (EventClassification)dto.Classification,
                status: (EventStatus)dto.Status,
                hasFees: dto.HasFees,
                isOnline: dto.IsOnline,
                programName: dto.ProgramName,
                format: dto.Format,
                schoolBookable: dto.SchoolBookable,
                ageMinimum: dto.AgeMinimum,
                ageMaximum: dto.AgeMaximum,
                organizationId: dto.Organization,
                locationId: dto.Location,
                contactId: dto.Contact
            );

            var created = await _eventLogic.CreateAsync(
                domain,
                targetAudiences: dto.TargetAudiences,
                topics: dto.Topics);

            var resultDto = MapToDto(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
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