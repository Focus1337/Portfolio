using Microsoft.AspNetCore.Mvc;
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

    public ContactController(IEmailService emailService, EmailConfiguration emailConfig, ApplicationContext context)
    {
        _emailService = emailService;
        _emailConfig = emailConfig;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index() =>
        View();

    [HttpPost]
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
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException)
            {
                ModelState.AddModelError("", "Mail service currently doesn't work. Try later.");
            }
        }

        return View("Index", model);
    }
}