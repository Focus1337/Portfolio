using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger) => 
        _logger = logger;

    public IActionResult Index() => 
        View();
    
    public IActionResult About() => 
        View();
    
    public IActionResult Blog() => 
        View();

    public IActionResult Work() => 
        View();
    
    public IActionResult Result(double a, double b) => 
        View( a + b);

    public IActionResult Privacy() => 
        View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => 
        View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
}