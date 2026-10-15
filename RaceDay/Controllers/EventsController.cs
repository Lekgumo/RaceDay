using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Middleware;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/events")]
[Tags("Events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EventsController(RaceDayDbContext db) => _db = db;

    /// <summary>Lists all events. Public.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var query = _db.Events.Include(e => e.Organiser).Include(e => e.Categories).AsQueryable();
        if (!string.IsNullOrEmpty(status)) query = query.Where(e => e.Status == status);

        var events = await query.Select(e => new EventResponseDto(
            e.EventId, e.Name, e.Description, e.Date, e.Location,
            e.MaxParticipants, e.Status, e.EventType,
            e.OrganiserId, e.Organiser!.FirstName + " " + e.Organiser.LastName,
            e.Categories.Select(c => new CategoryResponseDto(
                c.CategoryId, c.Name, c.Description, c.EntryFee,
                c.StartTime, c.MaxCapacity, 0)).ToList()
        )).ToListAsync();

        return Ok(events);
    }

    /// <summary>Gets a single event. Public.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await _db.Events.Include(x => x.Organiser)
            .Include(x => x.Categories).ThenInclude(c => c.Enrolments)
            .FirstOrDefaultAsync(x => x.EventId == id);

        if (e == null) return NotFound();

        return Ok(new EventResponseDto(
            e.EventId, e.Name, e.Description, e.Date, e.Location,
            e.MaxParticipants, e.Status, e.EventType,
            e.OrganiserId, e.Organiser!.FirstName + " " + e.Organiser.LastName,
            e.Categories.Select(c => new CategoryResponseDto(
                c.CategoryId, c.Name, c.Description, c.EntryFee,
                c.StartTime, c.MaxCapacity, c.Enrolments.Count)).ToList()
        ));
    }

    /// <summary>Creates a new event. Organiser only.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Organiser")) return StatusCode(403);

        var userId = HttpContext.GetUserId()!.Value;
        var ev = new Event
        {
            OrganiserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            Date = dto.Date,
            Location = dto.Location,
            MaxParticipants = dto.MaxParticipants,
            EventType = dto.EventType,
            Status = "Draft"
        };

        if (dto.Categories != null)
            foreach (var c in dto.Categories)
                ev.Categories.Add(new Category
                {
                    Name = c.Name,
                    Description = c.Description,
                    EntryFee = c.EntryFee,
                    StartTime = c.StartTime,
                    MaxCapacity = c.MaxCapacity
                });

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = ev.EventId }, new { ev.EventId, ev.Name, ev.Status });
    }

    /// <summary>Updates an event. Organiser owner only.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Organiser")) return StatusCode(403);

        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != HttpContext.GetUserId()!.Value) return StatusCode(403);

        ev.Name = dto.Name;
        ev.Description = dto.Description;
        ev.Date = dto.Date;
        ev.Location = dto.Location;
        ev.MaxParticipants = dto.MaxParticipants;
        ev.Status = dto.Status;
        ev.EventType = dto.EventType;
        ev.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ev);
    }

    /// <summary>Deletes an event. Organiser owner only.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Organiser")) return StatusCode(403);

        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != HttpContext.GetUserId()!.Value) return StatusCode(403);

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Gets the latest weather forecast for an event. Public.</summary>
    [HttpGet("{id}/weather")]
    public async Task<IActionResult> GetWeather(int id)
    {
        if (!await _db.Events.AnyAsync(e => e.EventId == id))
            return NotFound(new { error = "Event not found." });

        var forecast = await _db.WeatherForecasts
            .Where(w => w.EventId == id)
            .OrderByDescending(w => w.ForecastDate)
            .FirstOrDefaultAsync();

        if (forecast == null)
            return NotFound(new { error = "No forecast available for this event." });

        return Ok(new
        {
            forecast.ForecastId,
            forecast.ForecastDate,
            forecast.TemperatureHigh,
            forecast.TemperatureLow,
            forecast.Condition,
            forecast.WindSpeed,
            forecast.Humidity,
            forecast.LastUpdated
        });
    }
}