using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Middleware;
using RaceDay.Models;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/events/{eventId}/categories")]
[Tags("Categories")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public CategoriesController(RaceDayDbContext db) => _db = db;

    /// <summary>Lists categories for an event. Public.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(int eventId)
    {
        if (!await _db.Events.AnyAsync(e => e.EventId == eventId)) return NotFound();

        var cats = await _db.Categories.Where(c => c.EventId == eventId)
            .Select(c => new CategoryResponseDto(
                c.CategoryId, c.Name, c.Description, c.EntryFee,
                c.StartTime, c.MaxCapacity, c.Enrolments.Count))
            .ToListAsync();

        return Ok(cats);
    }

    /// <summary>Creates a category. Organiser owner only.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(int eventId, [FromBody] CreateCategoryDto dto)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        if (!HttpContext.IsInRole("Organiser")) return StatusCode(403);

        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != HttpContext.GetUserId()!.Value) return StatusCode(403);

        var cat = new Category
        {
            EventId = eventId,
            Name = dto.Name,
            Description = dto.Description,
            EntryFee = dto.EntryFee,
            StartTime = dto.StartTime,
            MaxCapacity = dto.MaxCapacity
        };

        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { eventId }, new { cat.CategoryId, cat.Name });
    }
}