using Microsoft.AspNetCore.Identity;
using Portfolio.Entity;

namespace Portfolio.DataAccess.Repository;

public class DbInitializer
{
    public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        const string ownerEmail = "screamfocus@gmail.com";
        const string password = "123456";

        if (await roleManager.FindByNameAsync("admin") == null)
            await roleManager.CreateAsync(new IdentityRole("admin"));

        if (await roleManager.FindByNameAsync("moderator") == null)
            await roleManager.CreateAsync(new IdentityRole("moderator"));
        
        if (await roleManager.FindByNameAsync("user") == null)
            await roleManager.CreateAsync(new IdentityRole("user"));
        
        if (await roleManager.FindByNameAsync("owner") == null)
            await roleManager.CreateAsync(new IdentityRole("owner"));

        if (await userManager.FindByNameAsync(ownerEmail) == null)
        {
            var owner = new User
            {
                Email = ownerEmail, UserName = ownerEmail, Name = "Focus", LastName = "Owner",
                RegisterDate = DateTime.Now
            };
            var result = await userManager.CreateAsync(owner, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(owner, "owner");
                await userManager.AddToRoleAsync(owner, "admin");
                await userManager.AddToRoleAsync(owner, "user");
                await userManager.AddToRoleAsync(owner, "moderator");
            }
        }
    }
}