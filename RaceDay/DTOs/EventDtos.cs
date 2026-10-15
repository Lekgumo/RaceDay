using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs;

public record CreateEventDto(
    [Required] string Name,
    string? Description,
    [Required] DateTime Date,
    [Required] string Location,
    int MaxParticipants,
    [Required] string EventType,
    List<CreateCategoryDto>? Categories
);

public record UpdateEventDto(
    string Name, string? Description, DateTime Date, string Location,
    int MaxParticipants, string Status, string EventType
);

public record EventResponseDto(
    int EventId, string Name, string? Description, DateTime Date,
    string Location, int MaxParticipants, string Status, string EventType,
    int OrganiserId, string OrganiserName, List<CategoryResponseDto> Categories
);

public record CreateCategoryDto(
    [Required] string Name, string? Description,
    decimal EntryFee, TimeSpan? StartTime, int MaxCapacity
);

public record CategoryResponseDto(
    int CategoryId, string Name, string? Description,
    decimal EntryFee, TimeSpan? StartTime, int MaxCapacity, int EnrolledCount
);