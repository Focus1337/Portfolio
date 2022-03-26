using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Controllers;

public class BlogController: Controller
{
    private readonly ILogger<BlogController> _logger;

    public BlogController(ILogger<BlogController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index() => View();

    public IActionResult Detail() => View();
}