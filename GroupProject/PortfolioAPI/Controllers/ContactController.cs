using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Misc.Services.EmailSender;
using PortfolioAPI.Models;

namespace PortfolioAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly EmailConfiguration _emailConfig;
    private readonly IEmailService _emailService;

    public ContactController(ApplicationContext context, EmailConfiguration emailConfig, IEmailService emailService)
    {
        _context = context;
        _emailConfig = emailConfig;
        _emailService = emailService;
    }

    // POST: api/Contact
    // [HttpPost]
    // public IActionResult Send([Bind("Name,Email,Subject,Message")] RequestDTO requestDto)
    // {
    //     var message = new Message(new[] {_emailConfig.From}, $"Contact form: {requestDto.Subject}",
    //         $"Name: {requestDto.Name}\nEmail: {requestDto.Email}\n\n{requestDto.Message}");
    //     _emailService.SendEmail(message);
    //
    //     _context.Requests.Add(new Request
    //     {
    //         Id = Guid.NewGuid(),
    //         Name = requestDto.Name,
    //         Email = requestDto.Email,
    //         Subject = requestDto.Subject,
    //         Message = requestDto.Message
    //     });
    //     _context.SaveChanges();
    //     
    //     return Ok("sent successfully");
    // }

    // GET: api/Contact
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RequestDto>>> GetRequests()
    {
        return await _context.Requests
            .Select(x => RequestToDto(x))
            .ToListAsync();
    }

    // GET: api/Contact/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RequestDto>> GetRequest(Guid id)
    {
        var request = await _context.Requests.FindAsync(id);

        if (request == null)
            return NotFound();

        return RequestToDto(request);
    }

    // POST: api/Contact
    [HttpPost]
    public async Task<ActionResult<RequestDto>> Send(RequestDto requestDto)
    {
        var message = new Message(new[] {_emailConfig.From}, $"Contact form: {requestDto.Subject}",
            $"Name: {requestDto.Name}\nEmail: {requestDto.Email}\n\n{requestDto.Message}");
        await _emailService.SendEmailAsync(message);

        var request = new Request
        {
            Id = Guid.NewGuid(),
            Name = requestDto.Name,
            Email = requestDto.Email,
            Subject = requestDto.Subject,
            Message = requestDto.Message
        };
        _context.Requests.Add(request);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRequest),
            new {id = request.Id},
            RequestToDto(request));
    }

    private static RequestDto RequestToDto(Request request) =>
        new()
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message
        };
}