using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.Middleware;
using Portfolio.Misc.Services.EmailSender;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

#region Configure services
services.AddControllersWithViews();

services
    .AddSingleton(builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>())
    .AddScoped<IEmailService, EmailService>();

services.AddDbContext<ApplicationContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

services.AddIdentity<User, IdentityRole>(opts=> {
        opts.Password.RequiredLength = 6;
        opts.Password.RequireNonAlphanumeric = false; 
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<ApplicationContext>();

services.ConfigureApplicationCookie(options =>
{
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

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// app
// .UseMiddleware<ContactMiddleware>()
// .UseMiddleware<RequestLoggerMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
#endregion