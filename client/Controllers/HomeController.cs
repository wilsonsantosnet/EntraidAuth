using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace client.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(ILogger<HomeController> logger,ITokenAcquisition tokenAcquisition,IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "api://46699347-22ae-4a3c-a1d0-73983c86f13b/access.all" });
        var responseA = await BackAD(accessToken, "http://localhost:5237");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    private async Task<string> BackAD(string accessToken, string API)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri(API);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.GetAsync($"WeatherForecast");
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }
}
