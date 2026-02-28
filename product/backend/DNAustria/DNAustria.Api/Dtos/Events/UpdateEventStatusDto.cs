using DNAustria.Domain;

namespace DNAustria.Api.Dtos.Events;

public class UpdateEventStatusDto
{
    public EventStatus Status { get; set; }
}