namespace RaceDay.DTOs;

public record CreateEnrolmentDto(int EventId, int CategoryId);

public record EnrolmentResponseDto(
    int EnrolmentId, int EventId, string EventName,
    int CategoryId, string CategoryName,
    DateTime EnrolmentDate, string Status, int? BibNumber
);