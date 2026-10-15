using System.Text.Json;

namespace RaceDay.Services;

public interface ISessionService
{
    void SetUser(HttpContext context, int userId, string role, string email, string name);
    (int UserId, string Role, string Email, string Name)? GetUser(HttpContext context);
    void Clear(HttpContext context);
}

public class SessionService : ISessionService
{
    private const string SessionKey = "RaceDayUser";

    public void SetUser(HttpContext context, int userId, string role, string email, string name)
    {
        var data = JsonSerializer.Serialize(new { userId, role, email, name });
        context.Session.SetString(SessionKey, data);
    }

    public (int UserId, string Role, string Email, string Name)? GetUser(HttpContext context)
    {
        var json = context.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json)) return null;

        var doc = JsonDocument.Parse(json);
        return (
            doc.RootElement.GetProperty("userId").GetInt32(),
            doc.RootElement.GetProperty("role").GetString()!,
            doc.RootElement.GetProperty("email").GetString()!,
            doc.RootElement.GetProperty("name").GetString()!
        );
    }

    public void Clear(HttpContext context) => context.Session.Clear();
}