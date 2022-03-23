using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager) =>
        _userManager = userManager;

    public IActionResult Index() =>
        View(_userManager.Users.ToList());

    public IActionResult Create() =>
        View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User {Email = model.Email, UserName = model.Email};
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

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
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
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
        }

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                IdentityResult result = 
                    await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
            }
        }
        return View(model);
    }
}