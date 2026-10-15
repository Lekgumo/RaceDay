using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

public class WeatherForecast
{
    [Key]
    public int ForecastId { get; set; }

    [Required]
    public int EventId { get; set; }

    public DateTime ForecastDate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TemperatureHigh { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TemperatureLow { get; set; }

    [MaxLength(100)]
    public string? Condition { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? WindSpeed { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? Humidity { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}