using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Controllers;

[Authorize(Roles = "user")]
public class UsersController : Controller
{
    public IActionResult Index() =>
        View();
}