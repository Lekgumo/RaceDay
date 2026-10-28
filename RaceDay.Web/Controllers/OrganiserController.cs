using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class OrganiserController : Controller
{
    private readonly ApiService _api;

    public OrganiserController(ApiService api) => _api = api;

    // GET: /Organiser/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        // Security check: Only Organisers allowed
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
        {
            return RedirectToAction("Login", "Auth");
        }

        var events = await _api.GetAsync<List<EventResponse>>("/api/events");

        // Filter to only show this organiser's events
        var myId = HttpContext.Session.GetInt32("UserId");
        var myEvents = events?.Where(e => e.OrganiserId == myId).ToList() ?? new List<EventResponse>();

        return View(myEvents);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Auth");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEventRequest request, IFormFile bannerImage)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Auth");

        // 1. Create the Event
        var response = await _api.PostAsync("/api/events", request);

        if (response.IsSuccessStatusCode)
        {
            // 2. If an image was uploaded, send it to the API
            if (bannerImage != null && bannerImage.Length > 0)
            {
                // Get the new Event ID from the API response
                var json = await response.Content.ReadAsStringAsync();
                var newEventDoc = System.Text.Json.JsonDocument.Parse(json);
                int newEventId = newEventDoc.RootElement.GetProperty("eventId").GetInt32();

                // Send the file to the API (Wrapped in try-catch to catch Azure errors)
                try
                {
                    var uploadResponse = await _api.PostFileAsync($"/api/events/{newEventId}/banner", bannerImage);

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        var errorMsg = await uploadResponse.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine("========================================");
                        System.Diagnostics.Debug.WriteLine($"UPLOAD FAILED (Status {uploadResponse.StatusCode}): {errorMsg}");
                        System.Diagnostics.Debug.WriteLine("========================================");

                        TempData["Error"] = "Event created, but image upload failed.";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("==================================================");
                    System.Diagnostics.Debug.WriteLine($"CRITICAL UPLOAD ERROR: {ex.Message}");
                    if (ex.InnerException != null)
                        System.Diagnostics.Debug.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine("==================================================");

                    TempData["Error"] = "Event created, but a critical error occurred during upload.";
                }
            }

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction("Dashboard");
        }

        ViewBag.Error = "Failed to create event.";
        return View(request);
    }
}