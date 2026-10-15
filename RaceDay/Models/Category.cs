using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal EntryFee { get; set; }

    public TimeSpan? StartTime { get; set; }

    public int MaxCapacity { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    public ICollection<Result> Results { get; set; } = new List<Result>();
}