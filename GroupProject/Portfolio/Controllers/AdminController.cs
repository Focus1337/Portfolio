using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

[Authorize(Roles = "admin, moderator")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(UserManager<User> userManager, ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Index() =>
        View(_userManager.Users.ToList());

    public IActionResult Create() =>
        View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                Email = model.Email, UserName = model.Email, Name = model.Name, LastName = model.LastName,
                RegisterDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");

                _logger.LogInformation("admin \"{Admin}\" created new user - {User}",
                    User.Identity!.Name, JsonConvert.SerializeObject(user.UserName));

                return RedirectToAction("Index");
            }
            else
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var model = new EditUserViewModel {Id = user.Id, Email = user.Email};
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                var oldEmail = user.Email;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("admin \"{Admin}\" edited user - {User} | New email: {New}",
                        User.Identity!.Name, JsonConvert.SerializeObject(oldEmail),
                        JsonConvert.SerializeObject(user.UserName));

                    return RedirectToAction("Index");
                }
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                _logger.LogInformation("admin \"{Admin}\" deleted user - {User}",
                    User.Identity!.Name, JsonConvert.SerializeObject(user.UserName));
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ChangePassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var model = new ChangePasswordViewModel {Id = user.Id, Email = user.Email};
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                var result =
                    await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("admin \"{Admin}\" changed password for user - {User}",
                        User.Identity!.Name, JsonConvert.SerializeObject(user.UserName));

                    return RedirectToAction("Index");
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
}