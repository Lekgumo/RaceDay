namespace RaceDay.Web.DTOs;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string Role);
public record LoginRequest(string Email, string Password);
public record LoginResponse(int UserId, string Role, string FirstName);
public record UserProfileResponse(int UserId, string FirstName, string LastName, string Email, string Role, DateTime DateRegistered);