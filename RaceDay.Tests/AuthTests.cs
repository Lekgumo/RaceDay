using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RaceDay.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public AuthTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var client = _factory.CreateClientWithCookies();
        var dto = new RegisterDto("Test", "User", $"t{Guid.NewGuid()}@x.com", "Password123", "Participant");
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithInvalidRole_Returns400()
    {
        var client = _factory.CreateClientWithCookies();
        var dto = new RegisterDto("Test", "User", $"b{Guid.NewGuid()}@x.com", "Password123", "Hacker");
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"dup{Guid.NewGuid()}@x.com";
        var dto = new RegisterDto("Test", "User", email, "Password123", "Participant");

        await client.PostAsJsonAsync("/api/auth/register", dto);
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"lw{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Test", "User", email, "CorrectPass123", "Participant"));

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "WrongPass"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"lok{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Test", "User", email, "CorrectPass123", "Organiser"));

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "CorrectPass123"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        var client = _factory.CreateClientWithCookies();
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto($"nobody{Guid.NewGuid()}@x.com", "Password123"));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}