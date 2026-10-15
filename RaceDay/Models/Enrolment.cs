using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

public class Enrolment
{
    public int EnrolmentId { get; set; }

    [Required]
    public int ParticipantId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(100)]
    public string? PaymentReference { get; set; }

    public int? BibNumber { get; set; }

    [ForeignKey(nameof(ParticipantId))]
    public User? Participant { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
}