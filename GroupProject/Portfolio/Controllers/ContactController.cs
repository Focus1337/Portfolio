using Microsoft.AspNetCore.Mvc;
using Portfolio.Misc.Services.EmailSender;

namespace Portfolio.Controllers;

public class ContactController : Controller
{
    private readonly IEmailService _emailService;

    // GET
    public ContactController(IEmailService emailService) =>
        _emailService = emailService;

    [HttpGet]
    public IActionResult Index() =>
        View();
    
    [HttpPost]
    public IActionResult Send()
    {
        var message = new Message(new string[] {"maniaclord@bk.ru"}, "Test email",
            "This is the content from our email.");
        _emailService.SendEmail(message);

        return Ok("sent successfully");
    }
}