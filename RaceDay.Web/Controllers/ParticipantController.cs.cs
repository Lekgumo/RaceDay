using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class ParticipantController : Controller
{
    private readonly ApiService _api;

    public ParticipantController(ApiService api) => _api = api;

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        // Check if logged in
        if (HttpContext.Session.GetString("UserRole") != "Participant")
        {
            return RedirectToAction("Login", "Auth");
        }

        var request = new CreateEnrolmentRequest(eventId, categoryId);
        var response = await _api.PostAsync("/api/enrolments", request);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "You have successfully enrolled!";
            return RedirectToAction("MyEnrolments");
        }

        // If error (e.g., already enrolled, or full)
        TempData["Error"] = "Failed to enrol. You might already be enrolled in this category.";
        return RedirectToAction("Details", "Events", new { id = eventId });
    }

    public async Task<IActionResult> MyEnrolments()
    {
        if (HttpContext.Session.GetString("UserRole") != "Participant")
        {
            return RedirectToAction("Login", "Auth");
        }

        var enrolments = await _api.GetAsync<List<EnrolmentResponse>>("/api/enrolments/me");
        return View(enrolments ?? new List<EnrolmentResponse>());
    }

    public async Task<IActionResult> MyResults()
    {
        if (HttpContext.Session.GetString("UserRole") != "Participant")
        {
            return RedirectToAction("Login", "Auth");
        }

        // ✨ FIX: Changed dynamic to System.Text.Json.JsonElement
        var results = await _api.GetAsync<List<System.Text.Json.JsonElement>>("/api/results/me");

        return View(results ?? new List<System.Text.Json.JsonElement>());
    }
}