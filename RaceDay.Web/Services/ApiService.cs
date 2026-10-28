using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private readonly IConfiguration _config;

    public ApiService(HttpClient http, IHttpContextAccessor ctx, IConfiguration config)
    {
        _http = http;
        _ctx = ctx;
        _config = config;

        var baseUrl = _config["ApiSettings:BaseUrl"] ?? "https://localhost:7064/";
        _http.BaseAddress = new Uri(baseUrl);
    }

    // ✨ FIX: ONLY forward the API's session cookie
    private void ForwardCookies()
    {
        var cookies = _ctx.HttpContext?.Request.Cookies;
        if (cookies != null && cookies.Count > 0)
        {
            // Look specifically for the API's default session cookie name
            var apiCookie = cookies.FirstOrDefault(c => c.Key == ".AspNetCore.Session");

            if (apiCookie.Key != null)
            {
                _http.DefaultRequestHeaders.Remove("Cookie");
                _http.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Session={apiCookie.Value}");
            }
        }
    }

    private void CaptureCookies(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                var parts = cookie.Split(';')[0].Split('=');
                if (parts.Length == 2)
                {
                    _ctx.HttpContext?.Response.Cookies.Append(parts[0], parts[1], new CookieOptions
                    {
                        HttpOnly = true,
                        IsEssential = true
                    });
                }
            }
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        ForwardCookies();
        return await _http.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T body)
    {
        ForwardCookies();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(endpoint, content);
        CaptureCookies(response);
        return response;
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T body)
    {
        ForwardCookies();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PutAsync(endpoint, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        ForwardCookies();
        return await _http.DeleteAsync(endpoint);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await GetAsync(endpoint);
        if (!response.IsSuccessStatusCode) return default;

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<HttpResponseMessage> PostFileAsync(string endpoint, IFormFile file)
    {
        ForwardCookies();
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        var response = await _http.PostAsync(endpoint, content);
        CaptureCookies(response);
        return response;
    }
}