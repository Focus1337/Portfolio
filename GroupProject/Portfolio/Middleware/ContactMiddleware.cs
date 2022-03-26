using Portfolio.DataAccess.Repository;

namespace Portfolio.Middleware;

public class ContactMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ContactMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ContactMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context, ApplicationContext applicationContext)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            foreach (var e in applicationContext.Requests)
                _logger.LogInformation(
                    "Request list: Guid: {Id} | Name: {Name} | Email: {Email}| Subject: {Subject} | Message: {Message}",
                    e.Id, e.Name, e.Email, e.Subject, e.Message);
        }
    }
}