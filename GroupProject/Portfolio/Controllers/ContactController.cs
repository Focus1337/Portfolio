using Microsoft.AspNetCore.Mvc;
using Portfolio.Misc.Services.EmailSender;
using Portfolio.Models;

namespace Portfolio.Controllers;

public class ContactController : Controller
{
    private readonly IEmailService _emailService;
    private readonly EmailConfiguration _emailConfig;

    public ContactController(IEmailService emailService, EmailConfiguration emailConfig)
    {
        _emailService = emailService;
        _emailConfig = emailConfig;
    }

    [HttpGet]
    public IActionResult Index() =>
        View();

    [HttpPost]
    public IActionResult Send([Bind("Name,Email,Subject,Message")] Contact contact)
    {
        var message = new Message(new[] {_emailConfig.From}, $"Contact form: {contact.Subject}",
            $"Name: {contact.Name}\nEmail: {contact.Email}\n\n{contact.Message}");
        _emailService.SendEmail(message);
        
         return Ok("sent successfully");
    }
}