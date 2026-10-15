using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTOs;

public record RegisterDto(
    [Required] string FirstName,
    [Required] string LastName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string Role
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record UserProfileDto(int UserId, string FirstName, string LastName, string Email, string Role, DateTime DateRegistered);

public record UpdateProfileDto(string FirstName, string LastName, string Email);