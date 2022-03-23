using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Entity;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register() =>
        View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User {Email = model.Email, UserName = model.Email};
            // добавляем пользователя
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                // установка куки
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
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

    private static string RemovePrefixFromUrl(string url) =>
        /* http://localhost:53773 only -> /Toys/Edit/1 */
        new Regex(@"(.*):(\d*)").Replace(url, string.Empty);

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var url = Request.Headers["Referer"].ToString();
        var returnUrl = RemovePrefixFromUrl(url);

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return View(new LoginViewModel
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ModelState.Remove("ExternalLogins");
        ModelState.Remove("ReturnUrl");
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            if (result.RequiresTwoFactor)
            {
                Console.WriteLine("Account requires 2fa");
                // return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                Console.WriteLine("Account is in lockout");
                // return RedirectToPage("./Lockout");
            }
            else
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // удаляем аутентификационные куки
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult ExternalLogins() =>
        View();
}