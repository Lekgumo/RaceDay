namespace RaceDay.Web.DTOs;

public record CreateResultRequest(
    int ParticipantId, int CategoryId,
    TimeSpan? FinishTime, int? Position, string Status, string? Notes);

public record ResultResponse(
    int ResultId, string ParticipantName, string CategoryName,
    TimeSpan? FinishTime, int? Position, string Status, string? Notes);