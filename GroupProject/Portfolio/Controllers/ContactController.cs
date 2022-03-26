using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.Misc.Services.EmailSender;
using Portfolio.ViewModels;

namespace Portfolio.Controllers;

public class ContactController : Controller
{
    private readonly IEmailService _emailService;
    private readonly EmailConfiguration _emailConfig;
    private readonly ApplicationContext _context;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IEmailService emailService, EmailConfiguration emailConfig, ApplicationContext context,
        ILogger<ContactController> logger)
    {
        _emailService = emailService;
        _emailConfig = emailConfig;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() =>
        View();

    [HttpPost]
    [Authorize(Roles="user")]
    [ValidateAntiForgeryToken]
    public IActionResult Send(RequestViewModel model)
    {
        if (ModelState.IsValid)
        {
            var message = new Message(new[] {_emailConfig.From}, $"Contact form: {model.Subject}",
                $"Name: {model.Name}\nEmail: {model.Email}\n\n{model.Message}");
            try
            {
                _emailService.SendEmail(message);

                _context.Requests.Add(new Request
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message
                });
                _context.SaveChanges();

                ModelState.AddModelError("", "Mail sent successfully!");

                _logger.LogInformation("User \"{User}\" sent mail | Subject: {Subject}", User.Identity!.Name,
                    model.Subject);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException ex)
            {
                ModelState.AddModelError("", "Mail service currently doesn't work. Try later.");
                _logger.LogWarning(ex, "User \"{User}\" failed to send a mail", User.Identity!.Name);
            }
        }

        return View("Index", model);
    }
}