using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Controllers;

[Authorize(Roles = "user")]
public class ProfileController : Controller
{
    public IActionResult Index() =>
        View();
    
    [HttpGet]
    public IActionResult Edit() =>
        View();
}