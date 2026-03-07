using DNAustria.Domain;

namespace DNAustria.Api.Dtos.Events;

public class UpdateEventStatusDto
{
    public required EventStatus Status { get; init; }
}