namespace Portfolio.Middleware;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    
    public RequestLoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestLoggerMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        finally
        {
            context.Request.Headers.TryGetValue("Accept", out var contentType);

            _logger.LogInformation(
                "OS: {OS} | Browser: {Browser} | Method: {Method}| Ip address: {Ip address} | Content type: {ContentType} | Protocol: {Protocol}",
                GetUserPlatform(context), GetUserBrowser(context), context.Request.Method,
                context.Connection.RemoteIpAddress?.MapToIPv4(), contentType, context.Request.Protocol);

            // using (var reader = new StreamReader(context.Request.Body))
            // {
            //     var body = await reader.ReadToEndAsync();
            //     Console.WriteLine($"Body: {body}");
            // }

            Console.WriteLine($"Query: {context.Request.QueryString.Value}");
        }
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