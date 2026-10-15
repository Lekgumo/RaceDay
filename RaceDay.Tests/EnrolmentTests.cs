using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using RaceDay.DTOs;
using Xunit;

namespace RaceDay.Tests;

public class EnrolmentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public EnrolmentTests(CustomWebApplicationFactory factory) => _factory = factory;

    private static int ExtractEventId(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("eventId", out var a)) return a.GetInt32();
        if (root.TryGetProperty("EventId", out var b)) return b.GetInt32();
        throw new Exception($"No eventId found in response: {json}");
    }

    [Fact]
    public async Task Participant_CanEnrol_Returns201()
    {
        // --- Organiser: one client for whole session ---
        var orgClient = _factory.CreateClientWithCookies();
        var orgEmail = $"org{Guid.NewGuid()}@x.com";
        await orgClient.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Org", "User", orgEmail, "Password123", "Organiser"));
        await orgClient.PostAsJsonAsync("/api/auth/login",
            new LoginDto(orgEmail, "Password123"));

        var createResp = await orgClient.PostAsJsonAsync("/api/events",
            new CreateEventDto("Test Event", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 100, "Run",
                new List<CreateCategoryDto> { new("10km", "Ten km", 100m, null, 50) }));

        var createJson = await createResp.Content.ReadAsStringAsync();
        createResp.StatusCode.Should().Be(HttpStatusCode.Created, $"Response: {createJson}");

        int eventId = ExtractEventId(createJson);

        var cats = await orgClient.GetFromJsonAsync<List<CategoryResponseDto>>(
            $"/api/events/{eventId}/categories");
        int categoryId = cats![0].CategoryId;

        // --- Participant: one client for whole session ---
        var pClient = _factory.CreateClientWithCookies();
        var pEmail = $"p{Guid.NewGuid()}@x.com";
        await pClient.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("P", "User", pEmail, "Password123", "Participant"));
        await pClient.PostAsJsonAsync("/api/auth/login",
            new LoginDto(pEmail, "Password123"));

        var enrolResp = await pClient.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentDto(eventId, categoryId));

        var enrolJson = await enrolResp.Content.ReadAsStringAsync();
        enrolResp.StatusCode.Should().Be(HttpStatusCode.Created, $"Response: {enrolJson}");
    }

    [Fact]
    public async Task Organiser_CannotEnrol_Returns403()
    {
        var client = _factory.CreateClientWithCookies();
        var email = $"org2{Guid.NewGuid()}@x.com";
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Org", "User", email, "Password123", "Organiser"));
        await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto(email, "Password123"));

        var response = await client.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentDto(1, 1));

        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, $"Response: {body}");
    }

    [Fact]
    public async Task Participant_CannotEnrolTwiceInSameCategory_Returns409()
    {
        var orgClient = _factory.CreateClientWithCookies();
        var orgEmail = $"org{Guid.NewGuid()}@x.com";
        await orgClient.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("Org", "User", orgEmail, "Password123", "Organiser"));
        await orgClient.PostAsJsonAsync("/api/auth/login",
            new LoginDto(orgEmail, "Password123"));

        var createResp = await orgClient.PostAsJsonAsync("/api/events",
            new CreateEventDto("Dup Test", "Desc", DateTime.UtcNow.AddDays(30),
                "Durban", 100, "Run",
                new List<CreateCategoryDto> { new("10km", "Ten km", 100m, null, 50) }));

        var createJson = await createResp.Content.ReadAsStringAsync();
        createResp.StatusCode.Should().Be(HttpStatusCode.Created, $"Response: {createJson}");

        int eventId = ExtractEventId(createJson);

        var cats = await orgClient.GetFromJsonAsync<List<CategoryResponseDto>>(
            $"/api/events/{eventId}/categories");
        int categoryId = cats![0].CategoryId;

        var pClient = _factory.CreateClientWithCookies();
        var pEmail = $"p2{Guid.NewGuid()}@x.com";
        await pClient.PostAsJsonAsync("/api/auth/register",
            new RegisterDto("P", "User", pEmail, "Password123", "Participant"));
        await pClient.PostAsJsonAsync("/api/auth/login",
            new LoginDto(pEmail, "Password123"));

        var first = await pClient.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentDto(eventId, categoryId));
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await pClient.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentDto(eventId, categoryId));
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}