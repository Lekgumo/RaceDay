using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RaceDay.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class RoleAccessTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public RoleAccessTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Participant_CannotCreateEvent_Returns403()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"p{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("P", "User", email, "Password123", "Participant"));
        await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "Password123"));

        var response = await client.PostAsJsonAsync("/api/events", new CreateEventDto(
            "Test", "Desc", DateTime.UtcNow.AddDays(30),
            "Cape Town", 100, "Run", null));

        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, $"Response: {body}");
    }

    [Fact]
    public async Task Unauthenticated_UserCannotCreateEvent_Returns401()
    {
        var client = _factory.CreateClientWithCookies();
        var response = await client.PostAsJsonAsync("/api/events", new CreateEventDto(
            "Test", "Desc", DateTime.UtcNow.AddDays(30),
            "Cape Town", 100, "Run", null));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unauthenticated_UserCannotAccess_Profile_Returns401()
    {
        var client = _factory.CreateClientWithCookies();
        var response = await client.GetAsync("/api/users/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Organiser_CannotAccessEnrolmentsMe_Returns403()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"org{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Org", "User", email, "Password123", "Organiser"));
        await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "Password123"));

        var response = await client.GetAsync("/api/enrolments/me");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, $"Response: {body}");
    }

    [Fact]
    public async Task Participant_CannotAccessResultsRecording_Returns403()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"p3{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("P", "User", email, "Password123", "Participant"));
        await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "Password123"));

        var response = await client.PostAsJsonAsync("/api/events/1/results",
            new List<CreateResultDto>
            {
                new(1, 1, TimeSpan.FromHours(2), 1, "Finished", null)
            });

        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, $"Response: {body}");
    }
}