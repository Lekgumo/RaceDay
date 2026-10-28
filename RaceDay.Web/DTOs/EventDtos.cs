namespace RaceDay.Web.DTOs;

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal EntryFee { get; set; }
    public TimeSpan? StartTime { get; set; }
    public int MaxCapacity { get; set; }
    public int EnrolledCount { get; set; }
}

public class EventResponse
{
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public int OrganiserId { get; set; }
    public string OrganiserName { get; set; } = string.Empty;
    public List<CategoryResponse> Categories { get; set; } = new();

    // ✨ ADDED THIS: The API is returning BannerUrl, so the DTO must have it too!
    public string? BannerUrl { get; set; }
}

public class CreateEventRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public string EventType { get; set; } = string.Empty;
    public List<CreateCategoryRequest>? Categories { get; set; }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal EntryFee { get; set; }
    public TimeSpan? StartTime { get; set; }
    public int MaxCapacity { get; set; }
}

public class CreateEnrolmentRequest
{
    public int EventId { get; set; }
    public int CategoryId { get; set; }

    public CreateEnrolmentRequest(int eventId, int categoryId)
    {
        EventId = eventId;
        CategoryId = categoryId;
    }
}

public class EnrolmentResponse
{
    public int EnrolmentId { get; set; }
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime EnrolmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? BibNumber { get; set; }
}