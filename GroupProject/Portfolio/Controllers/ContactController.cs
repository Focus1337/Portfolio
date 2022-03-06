using Microsoft.AspNetCore.Mvc;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.Misc.Services.EmailSender;
using Portfolio.Models;

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
    public IActionResult Send([Bind("Name,Email,Subject,Message")] Contact contact)
    {
        var message = new Message(new[] {_emailConfig.From}, $"Contact form: {contact.Subject}",
            $"Name: {contact.Name}\nEmail: {contact.Email}\n\n{contact.Message}");
        _emailService.SendEmail(message);

        _context.Requests.Add(new Request
        {
            Id = Guid.NewGuid(),
            Name = contact.Name,
            Email = contact.Email,
            Subject = contact.Subject,
            Message = contact.Message
        });
        _context.SaveChanges();
        
        return Ok("sent successfully");
    }
}