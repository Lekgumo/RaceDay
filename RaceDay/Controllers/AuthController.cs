using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Models;
using RaceDay.Services;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/auth")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ISessionService _session;

    public AuthController(RaceDayDbContext db, IPasswordHasher hasher, ISessionService session)
    {
        _db = db; _hasher = hasher; _session = session;
    }

    /// <summary>Registers a new user as Organiser or Participant.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (dto.Role != "Organiser" && dto.Role != "Participant")
            return BadRequest(new { error = "Role must be 'Organiser' or 'Participant'." });

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { error = "Email already registered." });

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = dto.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return StatusCode(201, new { userId = user.UserId, email = user.Email, role = user.Role });
    }

    /// <summary>Logs in a user and establishes a session.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid email or password." });

        _session.SetUser(HttpContext, user.UserId, user.Role, user.Email, $"{user.FirstName} {user.LastName}");
        return Ok(new { userId = user.UserId, role = user.Role, firstName = user.FirstName });
    }

    /// <summary>Logs out the current user.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        _session.Clear(HttpContext);
        return NoContent();
    }
}