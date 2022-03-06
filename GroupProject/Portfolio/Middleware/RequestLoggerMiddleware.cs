namespace Portfolio.Middleware;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggerMiddleware(RequestDelegate next) =>
        _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("\n");
        Console.WriteLine($"OS: {GetUserPlatform(context)}");
        Console.WriteLine($"Browser: {GetUserBrowser(context)}");
        Console.WriteLine($"Method: {context.Request.Method}");
        Console.WriteLine($"Ip address: {context.Connection.RemoteIpAddress?.MapToIPv4()}");
        
        context.Request.Headers.TryGetValue("Accept", out var contentType);
        Console.WriteLine($"Content type: {contentType}");
        Console.WriteLine($"Protocol: {context.Request.Protocol}");

        // using (var reader = new StreamReader(context.Request.Body))
        // {
        //     var body = await reader.ReadToEndAsync();
        //     Console.WriteLine($"Body: {body}");
        // }

        Console.WriteLine($"Query: {context.Request.QueryString.Value}");

        await _next.Invoke(context);
    }

    private static string GetUserPlatform(HttpContext context)
    {
        string userAgent = context.Request.Headers.UserAgent;

        if (userAgent.Contains("Mac OS"))
            return "Mac OS";
        if (userAgent.Contains("Windows NT"))
            return "Windows";

        return "Unknown OS";
    }
    
    private static string GetUserBrowser(HttpContext context)
    {
        string userAgent = context.Request.Headers.UserAgent;

        if (userAgent.Contains("Firefox"))
            return "Firefox";
        if (userAgent.Contains("Chrome"))
            return "Chrome";

        return "Unknown browser";
    }
}