using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Middleware;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/users")]
[Tags("User Profile")]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public UsersController(RaceDayDbContext db) => _db = db;

    /// <summary>Gets the authenticated user's profile.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        var userId = HttpContext.GetUserId()!.Value;
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        return Ok(new UserProfileDto(user.UserId, user.FirstName, user.LastName, user.Email, user.Role, user.DateRegistered));
    }

    /// <summary>Updates the authenticated user's profile.</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
    {
        if (!HttpContext.IsAuthenticated()) return Unauthorized();
        var userId = HttpContext.GetUserId()!.Value;
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        await _db.SaveChangesAsync();

        return Ok(new UserProfileDto(user.UserId, user.FirstName, user.LastName, user.Email, user.Role, user.DateRegistered));
    }
}