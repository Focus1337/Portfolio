using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

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

    public async Task<IActionResult> Index() =>
        View(await _context.Posts
            .OrderByDescending(date => date.Date)
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .ToListAsync());

    [HttpGet]
    public async Task<IActionResult> Detail(Guid postId) =>
        View(await _context.Posts
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(post => post.Id == postId));

    [Authorize(Roles = "user")]
    [HttpGet]
    public IActionResult Add() =>
        View();

    [Authorize(Roles = "user")]
    [HttpPost]
    public async Task<IActionResult> Add(AddBlogViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null)
                return NotFound();

            var tagList = (from tag in model.Tags.Split(';')
                where tag != ""
                select new Tag {Id = Guid.NewGuid(), Name = tag}).ToList();

            _context.Posts.Add(new Post
            {
                Title = model.Title, Text = model.Text, Tags = tagList, Author = user, AuthorId = user.Id,
                Date = DateTime.Now
            });
            await _context.SaveChangesAsync();

            ModelState.AddModelError("", "Posted successfully!");

            _logger.LogInformation("User \"{User}\" added new post - {Blog}", User.Identity!.Name,
                model.Title);
        }

        return View(model);
    }
}