namespace RaceDay.DTOs;

public record CreateResultDto(
    int ParticipantId, int CategoryId,
    TimeSpan? FinishTime, int? Position, string Status, string? Notes
);

public record ResultResponseDto(
    int ResultId, string ParticipantName, string CategoryName,
    TimeSpan? FinishTime, int? Position, string Status, string? Notes
);