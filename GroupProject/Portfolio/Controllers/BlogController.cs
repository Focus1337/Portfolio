using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

[Authorize(Roles = "user")]
public class BlogController : Controller
{
    private readonly ILogger<BlogController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationContext _context;

    public BlogController(ILogger<BlogController> logger, UserManager<User> userManager, ApplicationContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index() => View();

    public IActionResult Detail() => View();

    [HttpGet]
    public IActionResult Add() =>
        View();

    [HttpPost]
    public async Task<IActionResult> Add(AddBlogViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(User.Identity!.Name);
            if (user == null)
                return NotFound();
            
            _context.Posts.Add(new Post
            {
                Title = model.Title, Text = model.Text, Tags = model.Tags, Author = user, AuthorId = user.Id,
                Date = DateTime.Now
            });
            _context.SaveChanges();

            ModelState.AddModelError("", "Mail sent successfully!");

            _logger.LogInformation("User \"{User}\" added new blog - {Blog}", User.Identity!.Name,
                model.Title);
        }

        return View(model);
    }
}