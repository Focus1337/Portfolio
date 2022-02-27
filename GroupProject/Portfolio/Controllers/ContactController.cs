using Microsoft.AspNetCore.Mvc;
using Portfolio.Misc.Services.EmailSender;

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
    public IActionResult Send(string nameContact, string emailContact, string subjectContact, string messageContact)
    {
        var message = new Message(new[] {_emailConfig.From}, $"Contact form: {subjectContact}",
            $"Name: {nameContact}\nEmail: {emailContact}\n\n{messageContact}");
        _emailService.SendEmail(message);

        return Ok("sent successfully");
    }
}