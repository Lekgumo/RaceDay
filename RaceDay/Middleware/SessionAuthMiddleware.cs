using RaceDay.Services;

namespace RaceDay.Middleware;

public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;
    public SessionAuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
    {
        var user = sessionService.GetUser(context);
        if (user.HasValue)
        {
            context.Items["UserId"] = user.Value.UserId;
            context.Items["Role"] = user.Value.Role;
            context.Items["Email"] = user.Value.Email;
            context.Items["Name"] = user.Value.Name;
        }
        await _next(context);
    }
}

public static class SessionAuthExtensions
{
    public static int? GetUserId(this HttpContext context) => context.Items["UserId"] as int?;
    public static string? GetRole(this HttpContext context) => context.Items["Role"] as string;
    public static bool IsAuthenticated(this HttpContext context) => context.Items.ContainsKey("UserId");
    public static bool IsInRole(this HttpContext context, string role) => context.GetRole() == role;
}