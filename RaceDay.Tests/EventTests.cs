using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RaceDay.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class EventTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public EventTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Organiser_CanCreateEvent_Returns201()
    {
        // One client for the whole session
        var client = _factory.CreateClientWithCookies();

        var email = $"org{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Org", "User", email, "Password123", "Organiser"));

        var loginResp = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "Password123"));
        loginResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = new CreateEventDto(
            "Comrades 2026", "Ultimate human race",
            DateTime.UtcNow.AddMonths(6), "Durban", 25000, "Run", null);

        var response = await client.PostAsJsonAsync("/api/events", dto);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Response: {body}");
    }

    [Fact]
    public async Task GetEvents_IsPublic_Returns200()
    {
        var client = _factory.CreateClientWithCookies();
        var response = await client.GetAsync("/api/events");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEvent_WithInvalidId_Returns404()
    {
        var client = _factory.CreateClientWithCookies();
        var response = await client.GetAsync("/api/events/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Organiser_CanCreateEventWithCategories_Returns201()
    {
        var client = _factory.CreateClientWithCookies();

        var email = $"org{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Org", "User", email, "Password123", "Organiser"));
        await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "Password123"));

        var dto = new CreateEventDto(
            "Cape Town Cycle Tour 2026", "World's largest timed cycle event",
            DateTime.UtcNow.AddMonths(3), "Cape Town", 35000, "Cycle",
            new List<CreateCategoryDto>
            {
                new("Fun Ride 35km", "Leisure ride", 150m, null, 10000),
                new("Challenge 78km", "Full tour", 250m, null, 15000)
            });

        var response = await client.PostAsJsonAsync("/api/events", dto);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Response: {body}");
    }
}