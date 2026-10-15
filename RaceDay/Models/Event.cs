using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

public class Event
{
    public int EventId { get; set; }

    [Required]
    public int OrganiserId { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required, MaxLength(255)]
    public string Location { get; set; } = string.Empty;

    public int MaxParticipants { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Draft";

    [Required, MaxLength(20)]
    public string EventType { get; set; } = "Run";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(OrganiserId))]
    public User? Organiser { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
    public ICollection<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
}