using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

[Authorize(Roles = "user")]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager,
        ILogger<ProfileController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByNameAsync(User.Identity!.Name);
        return View(user);
    }


    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.FindByNameAsync(User.Identity!.Name);
        if (user == null)
            return NotFound();

        var model = new EditProfileViewModel
        {
            Email = user.Email, Name = user.Name, LastName = user.LastName, OldPassword = user.PasswordHash,
            NewPassword = user.PasswordHash
        };
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        ModelState.Remove("NewPassword");
        ModelState.Remove("OldPassword");
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);

            if (user != null)
            {
                var oldEmail = user.Email;
                if (user.Email != model.Email)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "You will be logged out!");
                }

                user.Name = model.Name;
                user.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Changes are saved successfully!");
                    _logger.LogInformation("User \"{User}\" edited personal info ({Email}, {Name}, {LastName})",
                        oldEmail, user.Email, user.Name, user.LastName);
                }
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            else
                ModelState.AddModelError(string.Empty, "User not found");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(EditProfileViewModel model)
    {
        ModelState.Remove("Email");
        ModelState.Remove("Name");
        ModelState.Remove("LastName");
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user != null)
            {
                var result =
                    await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                model = new EditProfileViewModel
                {
                    Email = user.Email, Name = user.Name, LastName = user.LastName, OldPassword = string.Empty,
                    NewPassword = string.Empty
                };

                if (result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Password updated successfully!");
                    _logger.LogInformation("User \"{User}\" changed his password", model.Email);
                }
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            else
                ModelState.AddModelError(string.Empty, "User not found");
        }

        return View("Edit", model);
    }

    [HttpGet]
    public async Task<IActionResult> Users(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            ViewBag.User = user;

            return View();
        }

        return RedirectToAction("PageNotFound", "Home");
    }
}