using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

[Authorize(Roles = "owner")]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RolesController> _logger;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,
        ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Index() =>
        View(_roleManager.Roles.ToList());

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                _logger.LogInformation("Owner \"{Owner}\" added new role - {Role}", User.Identity!.Name, name);
                return RedirectToAction("Index");
            }
            else
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(name);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Owner \"{Owner}\" deleted role - {Role}", User.Identity!.Name, role.Name);
            }
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(string userId)
    {
        // получаем пользователя
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            // получем список ролей пользователя
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(model);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string userId, List<string> roles)
    {
        // получаем пользователя
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // получем список ролей пользователя
            var userRoles = await _userManager.GetRolesAsync(user);
            // получаем все роли
            var allRoles = _roleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            var addedRoles = roles.Except(userRoles);
            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(roles);

            var result = await _userManager.AddToRolesAsync(user, addedRoles);
            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Owner \"{Owner}\" changed role for user - {User} (added: {Added})",
                    User.Identity!.Name, user.Email, addedRoles);
            }

            result = await _userManager.RemoveFromRolesAsync(user, removedRoles);
            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Owner \"{Owner}\" changed role for user - {User} (removed: {Removed})",
                    User.Identity!.Name, user.Email, removedRoles);
            }

            return RedirectToAction("Index", "Admin");
        }

        return NotFound();
    }
}