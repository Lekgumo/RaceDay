using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class EventsController : Controller
{
    private readonly ApiService _api;

    public EventsController(ApiService api) => _api = api;

    // GET: /Events
    public async Task<IActionResult> Index()
    {
        var events = await _api.GetAsync<List<EventResponse>>("/api/events");
        return View(events ?? new List<EventResponse>());
    }

    // GET: /Events/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var ev = await _api.GetAsync<EventResponse>($"/api/events/{id}");

        if (ev == null)
            return NotFound();

        return View(ev);
    }

    // ==========================================
    // ✨ NEW METHODS FOR CATEGORIES
    // ==========================================

    [HttpGet]
    public IActionResult CreateCategory(int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Auth");

        ViewBag.EventId = eventId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(int eventId, CreateCategoryRequest request)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Auth");

        var response = await _api.PostAsync($"/api/events/{eventId}/categories", request);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Category added successfully!";
            return RedirectToAction("Details", new { id = eventId });
        }

        ViewBag.Error = "Failed to add category.";
        ViewBag.EventId = eventId;
        return View(request);
    }
}