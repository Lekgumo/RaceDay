using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RaceDay.Data;
using RaceDay.Middleware;
using RaceDay.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ISessionService, SessionService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaceDay API",
        Version = "v1",
        Description = "RESTful API for the RaceDay event management system - Part 2"
    });
});

var app = builder.Build();

// Auto-apply migrations ONLY when using a relational database
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();

// Only migrate when a relational provider (SQL Server) is registered
if (db.Database.IsRelational())
{
db.Database.Migrate();
}
}

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay API v1");
c.RoutePrefix = "swagger";
});
}

app.UseSession();
app.UseMiddleware<SessionAuthMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }   // ← MUST be at the bottom