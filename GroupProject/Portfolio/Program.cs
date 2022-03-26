using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.Misc.Services.EmailSender;

using NLog;
using NLog.Web;
using Portfolio.Middleware;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


try
{
    #region Configure services

    services.AddControllersWithViews();

// NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    services
        .AddSingleton(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>())
        .AddScoped<IEmailService, EmailService>();

    services.AddDbContext<ApplicationContext>(opts =>
        opts.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

    services.AddIdentity<User, IdentityRole>(opts =>
        {
            opts.Password.RequiredLength = 6;
            opts.Password.RequireNonAlphanumeric = false;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<ApplicationContext>();

    services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = new PathString("/Auth/Login");
        options.AccessDeniedPath = new PathString("/Home/AccessDenied");
    });

    #endregion

    #region Configure app and middleware

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app
            .UseExceptionHandler("/Home/Error")
            .UseHsts();
    }

    app.Use(async (context, next) =>
    {
        await next();
        if (context.Response.StatusCode == 404)
        {
            context.Request.Path = "/Home/PageNotFound";
            await next();
        }
    });

    app
        .UseHttpsRedirection()
        .UseStaticFiles()
        .UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // app
    //     .UseMiddleware<ContactMiddleware>()
    //     .UseMiddleware<RequestLoggerMiddleware>();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();

    #endregion
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}