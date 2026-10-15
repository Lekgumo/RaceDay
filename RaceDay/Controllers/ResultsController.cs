using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Middleware;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api")]
[Tags("Results")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public ResultsController(RaceDayDbContext db) => _db = db;

    /// <summary>Records results for an event. Organiser owner only.</summary>
    [HttpPost("events/{eventId}/results")]
    public async Task<IActionResult> Create(int eventId, [FromBody] List<CreateResultDto> dtos)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Organiser")) return StatusCode(403);

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != HttpContext.GetUserId()!.Value) return StatusCode(403);

        foreach (var dto in dtos)
        {
            var existing = await _db.Results.FirstOrDefaultAsync(r =>
                r.EventId == eventId && r.ParticipantId == dto.ParticipantId && r.CategoryId == dto.CategoryId);

            if (existing != null)
            {
                existing.FinishTime = dto.FinishTime;
                existing.Position = dto.Position;
                existing.Status = dto.Status;
                existing.Notes = dto.Notes;
            }
            else
            {
                _db.Results.Add(new Result
                {
                    EventId = eventId,
                    ParticipantId = dto.ParticipantId,
                    CategoryId = dto.CategoryId,
                    FinishTime = dto.FinishTime,
                    Position = dto.Position,
                    Status = dto.Status,
                    Notes = dto.Notes
                });
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { count = dtos.Count });
    }

    /// <summary>Gets results for an event.</summary>
    [HttpGet("events/{eventId}/results")]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();

        var list = await _db.Results.Where(r => r.EventId == eventId)
            .Include(r => r.Participant).Include(r => r.Category)
            .Select(r => new ResultResponseDto(
                r.ResultId,
                r.Participant!.FirstName + " " + r.Participant.LastName,
                r.Category!.Name, r.FinishTime, r.Position, r.Status, r.Notes))
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>Gets the participant's own results.</summary>
    [HttpGet("results/me")]
    public async Task<IActionResult> GetMine()
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Participant")) return StatusCode(403);

        var userId = HttpContext.GetUserId()!.Value;
        var list = await _db.Results.Where(r => r.ParticipantId == userId)
            .Include(r => r.Category).Include(r => r.Event)
            .Select(r => new
            {
                r.ResultId,
                EventName = r.Event!.Name,
                CategoryName = r.Category!.Name,
                r.FinishTime,
                r.Position,
                r.Status,
                EventDate = r.Event.Date
            }).ToListAsync();

        return Ok(list);
    }
}