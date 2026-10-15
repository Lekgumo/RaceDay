using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDay.Data;

namespace RaceDay.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Fixed name — SAME database across all requests within a test class
    private readonly string _dbName = "RaceDayTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<RaceDayDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll(typeof(RaceDayDbContext));

            // Use the SAME database name for the lifetime of this factory
            services.AddDbContext<RaceDayDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }

    public HttpClient CreateClientWithCookies()
    {
        var cookieHandler = new CookieTrackingHandler(Server.CreateHandler());

        return new HttpClient(cookieHandler)
        {
            BaseAddress = new Uri("http://localhost")
        };
    }
}

internal class CookieTrackingHandler : DelegatingHandler
{
    private readonly System.Net.CookieContainer _cookies = new();

    public CookieTrackingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookieHeader = _cookies.GetCookieHeader(request.RequestUri!);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.Remove("Cookie");
            request.Headers.Add("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            foreach (var cookie in setCookies)
            {
                _cookies.SetCookies(request.RequestUri!, cookie);
            }
        }

        return response;
    }
}