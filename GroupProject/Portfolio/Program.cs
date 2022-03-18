using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio.DataAccess.Repository;
using Portfolio.Entity;
using Portfolio.Middleware;
using Portfolio.Misc.Services.EmailSender;

var builder = WebApplication.CreateBuilder(args);

#region Configure services
builder.Services.AddControllersWithViews();

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<ApplicationContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>();


#endregion

#region Configure app and middleware
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();    // подключение аутентификации
app.UseAuthorization();

// app.UseMiddleware<ContactMiddleware>();
// app.UseMiddleware<RequestLoggerMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
#endregion
