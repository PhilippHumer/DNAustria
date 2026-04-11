using DNAustria.Api.Dtos;
using DNAustria.Api.Dtos.Events;
using DNAustria.Domain;
using DNAustria.Logic;
using DNAustria.Logic.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DNAustria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventLogic eventLogic, ILLMLogic llmLogic, IConfiguration configuration, IEventExtractionService eventExtractionService) : ControllerBase
{
    private readonly IEventLogic _eventLogic = eventLogic ?? throw new ArgumentNullException(nameof(eventLogic));
    private readonly IEventExtractionService _eventExtractionService = eventExtractionService ?? throw new ArgumentNullException(nameof(eventExtractionService));
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;


    [HttpGet]
    public async Task<ActionResult<PagedEventsDto>> GetAll([FromQuery] string? name, [FromQuery] EventStatus? status, [FromQuery] int page = DefaultPage, [FromQuery] int pageSize = DefaultPageSize)
    {
        if (page < 1)
        {
            return BadRequest("Page must be at least 1.");
        }

        if (pageSize < 1)
        {
            return BadRequest("PageSize must be at least 1.");
        }

        var events = await _eventLogic.GetAllAsync(name, status, page, pageSize);

        return Ok(new PagedEventsDto
        {
            Items = events.Items.Select(e => MapToDto(e)).ToList(),
            Page = events.Page,
            PageSize = events.PageSize,
            TotalCount = events.TotalCount,
            TotalPages = events.TotalPages
        });
    }

    [HttpPost("llm")]
    public async Task<ActionResult<EventDto>> PostLlm([FromBody] LlmRequestDto? req)
    {
        try
        {
            var inputText = req?.GetInputText();
            if (string.IsNullOrWhiteSpace(inputText))
                return BadRequest("Text is required. Send JSON: { \"text\": \"...\" } (or legacy { \"prompt\": \"...\" }).");

            var extraction = await _eventExtractionService.ExtractEventAsync(inputText);
            if (!extraction.Success || extraction.Data is null)
                return BadRequest(extraction.ErrorMessage ?? "Failed to extract event data from LLM response.");

            var dto = extraction.Data;

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
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Failed to save event. Check referenced IDs (Organization/Location/Contact) and enum values. Details: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventDto>> GetById(int id)
    {
        var e = await _eventLogic.GetByIdAsync(id);
        if (e is null)
        {
            return NotFound();
        }

        var history = await _eventLogic.GetHistoryByEventIdAsync(id);

        return Ok(MapToDto(e, history));
    }

    [HttpPost]
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

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _eventLogic.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/status")]
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

    private static EventDto MapToDto(Event e, IReadOnlyList<EventHistoryEntry>? history = null)
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
            Topics = e.Topics.Select(x => (int)x.Topic).ToList(),
            History = history is null
                ? Array.Empty<EventHistoryDto>()
                : history.Select(x => new EventHistoryDto
                {
                    Action = x.Action,
                    CreatedAt = x.CreatedAt,
                    Username = x.Username
                }).ToList()
        };
    }
}
