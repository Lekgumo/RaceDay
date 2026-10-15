using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Middleware;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api")]
[Tags("Enrolments")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    /// <summary>Enrols participant in a category. Participant only.</summary>
    [HttpPost("enrolments")]
    public async Task<IActionResult> Create([FromBody] CreateEnrolmentDto dto)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Participant")) return StatusCode(403);

        var userId = HttpContext.GetUserId()!.Value;
        var ev = await _db.Events.FindAsync(dto.EventId);
        if (ev == null) return NotFound(new { error = "Event not found." });

        var cat = await _db.Categories.FirstOrDefaultAsync(c =>
            c.CategoryId == dto.CategoryId && c.EventId == dto.EventId);
        if (cat == null) return NotFound(new { error = "Category not found." });

        if (await _db.Enrolments.AnyAsync(e => e.ParticipantId == userId && e.CategoryId == dto.CategoryId))
            return Conflict(new { error = "Already enrolled." });

        var count = await _db.Enrolments.CountAsync(e => e.CategoryId == dto.CategoryId);
        if (count >= cat.MaxCapacity) return BadRequest(new { error = "Category full." });

        var enrolment = new Enrolment
        {
            ParticipantId = userId,
            CategoryId = dto.CategoryId,
            Status = "Pending",
            BibNumber = count + 1
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();
        return StatusCode(201, new { enrolmentId = enrolment.EnrolmentId, status = enrolment.Status, bibNumber = enrolment.BibNumber });
    }

    /// <summary>Gets the participant's enrolments.</summary>
    [HttpGet("enrolments/me")]
    public async Task<IActionResult> GetMine()
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Participant")) return StatusCode(403);

        var userId = HttpContext.GetUserId()!.Value;
        var list = await _db.Enrolments.Where(e => e.ParticipantId == userId)
            .Include(e => e.Category).ThenInclude(c => c!.Event)
            .Select(e => new EnrolmentResponseDto(
                e.EnrolmentId, e.Category!.EventId, e.Category.Event!.Name,
                e.CategoryId, e.Category.Name, e.EnrolmentDate, e.Status, e.BibNumber))
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>Gets all enrolments for an organiser's event.</summary>
    [HttpGet("events/{eventId}/enrolments")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Organiser")) return StatusCode(403);

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != HttpContext.GetUserId()!.Value) return StatusCode(403);

        var list = await _db.Enrolments.Where(e => e.Category!.EventId == eventId)
            .Include(e => e.Participant).Include(e => e.Category)
            .Select(e => new
            {
                e.EnrolmentId,
                e.ParticipantId,
                ParticipantName = e.Participant!.FirstName + " " + e.Participant.LastName,
                e.CategoryId,
                CategoryName = e.Category!.Name,
                e.EnrolmentDate,
                e.Status,
                e.BibNumber
            }).ToListAsync();

        return Ok(list);
    }

    /// <summary>Cancels an enrolment. Participant owner only.</summary>
    [HttpDelete("enrolments/{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Participant")) return StatusCode(403);

        var en = await _db.Enrolments.FindAsync(id);
        if (en == null) return NotFound();
        if (en.ParticipantId != HttpContext.GetUserId()!.Value) return StatusCode(403);

        _db.Enrolments.Remove(en);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}