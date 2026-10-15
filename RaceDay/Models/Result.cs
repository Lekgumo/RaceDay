using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

public class Result
{
    public int ResultId { get; set; }

    [Required]
    public int ParticipantId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int EventId { get; set; }

    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "DNS";

    public string? Notes { get; set; }

    [ForeignKey(nameof(ParticipantId))]
    public User? Participant { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}