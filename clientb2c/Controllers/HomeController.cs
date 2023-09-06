using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using clientb2c.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;


namespace clientb2c.Controllers;

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
        
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "https://seedazb2c.onmicrosoft.com/e9e529e1-e85a-4bf2-8676-62c2075bc786/access.all" });
        var responseA = await BackAD(accessToken, "http://localhost:7283");
        
        ViewData["Name"] = User.Identity.Name;
        var gn = User.Claims.Where(_=>_.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").FirstOrDefault();
        ViewData["givenName"] = gn?.Value;
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
