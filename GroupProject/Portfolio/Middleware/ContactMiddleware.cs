using Portfolio.DataAccess.Repository;

namespace Portfolio.Middleware;

public class ContactMiddleware
{
    private readonly RequestDelegate _next;

    public ContactMiddleware(RequestDelegate next) =>
        _next = next;

    public async Task InvokeAsync(HttpContext context, ApplicationContext applicationContext)
    {
        Console.WriteLine("\nRequest list:");
        foreach (var e in applicationContext.Requests)
            Console.WriteLine(
                $"Guid: {e.Id} | Name: {e.Name} | Email: {e.Email}| Subject: {e.Subject} | Message: {e.Message}");
        Console.WriteLine("\n");

        await _next.Invoke(context);
    }
}