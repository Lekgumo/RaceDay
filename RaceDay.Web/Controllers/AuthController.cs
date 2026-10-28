using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;

    public AuthController(ApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _api.PostAsync("/api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            // Save to session
            HttpContext.Session.SetString("UserRole", result!.Role);
            HttpContext.Session.SetString("UserName", result.FirstName);
            HttpContext.Session.SetString("UserEmail", request.Email);
            HttpContext.Session.SetInt32("UserId", result.UserId);

            return RedirectToAction("Index", "Events");
        }

        ViewBag.Error = "Invalid email or password.";
        return View(request);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _api.PostAsync("/api/auth/register", request);

        if (response.IsSuccessStatusCode)
        {
            // Auto-login after register
            return RedirectToAction("Login");
        }

        ViewBag.Error = "Registration failed. Email might already be in use.";
        return View(request);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Events");
    }
}