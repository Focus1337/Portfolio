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

            var post = new Post
            {
                Id = Guid.NewGuid(), Title = model.Title, Text = model.Text, Author = user, AuthorId = user.Id,
                Date = DateTime.Now
            };

            var tagNames = model.Tags.Split(';');
            var tags = await _context.Tags.Where(tag => tagNames.Contains(tag.Name)).ToListAsync();
            var newTags = (from tagName in tagNames
                    where tags.All(tag => tag.Name != tagName.ToLowerInvariant()) && tagName != ""
                    select new Tag
                        {Id = Guid.NewGuid(), Name = tagName.ToLowerInvariant()})
                .ToList();

            _context.Tags.AddRange(newTags);
            await _context.SaveChangesAsync();

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            post.Tags = tags.Union(newTags).ToList();
            await _context.SaveChangesAsync();

            _logger.LogInformation("User \"{User}\" added new post - {Post}", User.Identity!.Name,
                model.Title);

            return RedirectToAction("Detail", new {postId = post.Id});
        }

        return View(model);
    }

    [Authorize(Roles = "user")]
    [Authorize(Roles = "admin")]
    [Authorize(Roles = "moderator")]
    [Authorize(Roles = "owner")]
    [HttpPost]
    public async Task<IActionResult> Delete(Guid postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
        _context.Posts.Remove(post!);
        await _context.SaveChangesAsync();
        _logger.LogInformation("User \"{User}\" deleted post - {PostName} (Id: {Id})",
            User.Identity!.Name,
            post!.Title, post.Id);

        return RedirectToAction("Index");
    }
}